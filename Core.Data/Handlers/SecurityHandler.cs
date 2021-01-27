using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.Data.Contexts;
using Core.Data.Models.Entities.Security;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Handlers
{
    /// <summary>
    /// Provides methods for security related purposes
    /// </summary>
    public class SecurityHandler
    {
        private CoreContext CoreContext { get; set; }

        public SecurityHandler(CoreContext coreContext)
        {
            CoreContext = coreContext ?? throw new ArgumentNullException(nameof(coreContext));
        }

        /// <summary>
        /// Creates the password hash and the salt of the password.
        /// </summary>
        /// <param name="password">The password</param>
        /// <param name="saltSize">The salt size</param>
        /// <param name="iterationCount">The number of iterations to use during generation</param>
        /// <param name="byteDerivationCount">The number of bytes to derive</param>
        /// <returns>A tuple which contains the password hash and the salt.</returns>
        public static Tuple<string, string> CreatePasswordHashAndSalt(string password, int saltSize = 512, int iterationCount = 10000, int byteDerivationCount = 128)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltSize) { IterationCount = iterationCount };
            var passwordHash = pbkdf2.GetBytes(byteDerivationCount);
            var salt = pbkdf2.Salt;
            var returnResult =
                new Tuple<string, string>(Convert.ToBase64String(passwordHash), Convert.ToBase64String(salt));

            return returnResult;
        }
        /// <summary>
        /// Checks to see if the given password is correct or not
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="password">The password to check</param>
        /// <param name="iterationCount">The number of iteration counts which was used when deriving the bytes using the guidelines posted in RFC 2898</param>
        /// <param name="byteDerivationCount">The number of bytes derived using the guidelines posted in RFC 2898</param>
        /// <returns>True if the password is correct; otherwise, false</returns>
        private bool IsPasswordCorrect(string userId, string password, int iterationCount = 10000, int byteDerivationCount = 128)
        {
            var user = CoreContext.Users.FirstOrDefault(p => p.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(user.Salt), iterationCount);

            var passwordHash = Convert.ToBase64String(pbkdf2.GetBytes(byteDerivationCount));
            var returnResult = passwordHash == user.PasswordHash;

            return returnResult;
        }
        /// <summary>
        /// Checks to see whether or not a user is authenticated
        /// </summary>
        /// <param name="context">The executing context</param>
        /// <param name="user">The user to check</param>
        /// <returns>True if the user is authenticated. Otherwise, false</returns>
        public bool IsAuthenticated(ActionExecutingContext context, out string user)
        {
            // step 1: Check if token and nonce are present.
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authorizationHeader.Count != 1)
            {
                HandleInvalidResult(ref context, "No Authorization header found or invalid Authorization header");
            }

            var authorizationToken = context.HttpContext.Request.Headers["Authorization"][0];
            if (context.HttpContext.Request.Headers["NOnce"].Count == 0) HandleInvalidResult(ref context, "NOnce not found");
            
            var nOnceToken = context.HttpContext.Request.Headers["NOnce"][0];

            if (string.IsNullOrEmpty(authorizationToken))
            {
                HandleInvalidResult(ref context, "No Authorization header found");
            }

            if (string.IsNullOrEmpty(nOnceToken))
            {
                HandleInvalidResult(ref context, "No NOnce header found");
            }

            // step 2: Check if the token and nOnce are valid
            // step 2a: for the nOnce, check whether or not it has already been deleted or if the ExpiryDateUtc has already passed
            // step 2b: for the token, check whether or not it has already been deleted or if the ExpiryDateUtc has already passed
            byte[] tokenInBytes = null;
            byte[] nOnceInBytes = null;
            try
            {
                tokenInBytes = Convert.FromBase64String(authorizationToken);
            }
            catch (Exception e)
            {
                HandleInvalidResult(ref context, "Passed in authorization token was not in a valid base 64 format");
            }
            try
            {
                nOnceInBytes = Convert.FromBase64String(nOnceToken);
            }
            catch (Exception e)
            {
                HandleInvalidResult(ref context, "Passed in nOnce token was not in a valid base 64 format");
            }

            var token = CoreContext.UserTokens
                .Where(p => p.IsDeleted == false && DateTime.Compare(DateTime.UtcNow, p.ExpiryDateUtc) < 0)
                .FirstOrDefault(p => p.Value == tokenInBytes);
            var nOnce = CoreContext.NOnces
                .Where(p => p.IsDeleted == false && DateTime.Compare(DateTime.UtcNow, p.ExpiryDateUtc) < 0)
                .FirstOrDefault(p => p.Value == nOnceInBytes);

            if (token == null || nOnce == null)
            {
                HandleInvalidResult(ref context, "Invalid Authorization header or NOnce header");
            }

            user = token?.UserId;
            return true;
        }
        /// <summary>
        /// Checks to see if the user has the correct security permission
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="permissionNameUserWasTryingToAccess">The permission name that the user was trying to access</param>
        /// <returns>True if the user is authorized. Otherwise, false</returns>
        public bool IsAuthorized(string userId, string permissionNameUserWasTryingToAccess)
        {
            // Step 1: get the user
            var user = CoreContext.Users
                .AsNoTracking()
                .Include(p => p.UserPermissions)
                .ThenInclude(q => q.Permission)
                .FirstOrDefault(p => p.Id == userId);

            // no permissions? Not authorized
            if (user?.UserPermissions == null)
            {
                return false;
            }

            // Step 2: If user has the permission OR is a "super admin", return true; otherwise, return false
            return user.UserPermissions.Any(userPermission => string.Equals(userPermission?.Permission?.Name,
                permissionNameUserWasTryingToAccess, StringComparison.CurrentCultureIgnoreCase)) ||
                   user.UserPermissions.Any(userPermission => string.Equals(userPermission?.Permission?.Name,
                       "Administrator.AllActions", StringComparison.CurrentCultureIgnoreCase));
        }
        /// <summary>
        /// Sets the status code based on the statusCode parameter and writes to the response body the bodyMessage parameter.
        /// </summary>
        /// <param name="context">The executing context</param>
        /// <param name="bodyMessage">The message to write to the response body</param>
        /// <param name="statusCode">The status code to use when returning the response</param>
        /// <param name="shouldThrowException">If set to true, an exception will be thrown and will be logged in the data store</param>
        private void HandleInvalidResult(ref ActionExecutingContext context, string bodyMessage, int statusCode = 401, bool shouldThrowException = true)
        {
            context.HttpContext.Response.StatusCode = statusCode;
            context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(bodyMessage), 0, bodyMessage.Length);

            if (shouldThrowException)
            {
                throw new Exception(bodyMessage);
            }
        }
        /// <summary>
        /// Forces the NOnce to be invalidated
        /// </summary>
        /// <param name="nOnce">The NOnce to invalidate</param>
        public void InvalidateNOnce(string nOnce)
        {
            var nOnceInBytes = Convert.FromBase64String(nOnce);
            var existingNOnce = CoreContext.NOnces.FirstOrDefault(p => p.Value == nOnceInBytes);
            if (existingNOnce == null) return;
            existingNOnce.IsDeleted = true;
            existingNOnce.LastUpdatedUtc = DateTime.UtcNow;
            CoreContext.Update(existingNOnce);
            CoreContext.SaveChanges();
        }
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="username">The desired username</param>
        /// <param name="password">The desired password</param>
        /// <param name="email">The desired email address</param>
        /// <param name="errorList">Out parameter. Gets populated if any error has occurred during user creation</param>
        /// <param name="saltSize">The size of the salt to use in hashing the user's password</param>
        /// <param name="iterationCount">The number of iterations to use during generation</param>
        /// <param name="byteDerivationCount">The number of bytes to derive</param>
        /// <returns></returns>
        public string CreateNewUser(string username, string password, string email, out List<string> errorList, int saltSize = 512, int iterationCount = 10000, int byteDerivationCount = 128)
        {
            errorList = new List<string>();
            // Step 0: null checkers
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));


            // validation checkers

            if (CoreContext.Users.Any(p => string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase)))
            {
                errorList.Add("Username already exists");
            }
            if (!InputValidationHandler.IsValidEmail(email))
            {
                errorList.Add("Invalid email address");
            }
            if (!InputValidationHandler.IsValidPassword(password, CoreContext))
            {
                errorList.Add("Invalid password");
            }

            if (errorList.Any())
            {
                return null;
            }

            // after validation is done, create the password hash and salt, and save these 2 
            // along with the rest of the passed in info.
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltSize) { IterationCount = iterationCount };
            byte[] hash = pbkdf2.GetBytes(byteDerivationCount);
            byte[] salt = pbkdf2.Salt;

            var user = new User();
            // add default values
            DataHelper<User>.AddDefaultUserValues(ref user);
            user.EmailAddress = email;
            user.PasswordHash = Convert.ToBase64String(hash);
            user.Salt = Convert.ToBase64String(salt);
            user.Username = username;
            user.Role = CoreContext.Roles.Single(p => p.Name == "Default");
            user.UserPermissions = CoreContext.Roles.Single(p => p.Name == "Default").RolePermissions;
            CoreContext.Users.Add(user);
            CoreContext.SaveChanges();
            return user.Id;
        }
        /// <summary>
        /// Authenticates the user based on the given credentials and token. The credentials parameter MUST follow a certain format, which is: base64(appid:&lt;appid&gt;\nusername:&lt;username&gt;\npassword:&lt;password&gt;)
        /// </summary>
        /// <param name="credentials">The user's credentials in the form 'base64(appid:&lt;appid&gt;\nusername:&lt;username&gt;\npassword:&lt;password&gt;)'</param>
        /// <param name="token">The token used to authenticate the user</param>
        /// <param name="errors">Out parameter. Gets populated if any error has occurred during user authentication</param>
        /// <returns>The token and NOnce pair if no error occurred. Otherwise, null</returns>
        public Tuple<string, string> Authenticate(string credentials, string token, out List<string> errors)
        {
            // credentials should be in the format base64(appid:<appid>\nusername:<username>\npassword:<password>)
            errors = new List<string>();

            // Step 1: credentials verification
            var credentialsAsString = Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
            var credentialsArray = credentialsAsString.Split("\n");
            if (credentialsArray.Length != 3)
            {
                errors.Add(
                    $@"Token had {credentialsArray.Length} segment(s); expected 3 segments separated by a newline (\n) in the format 'appid:<appid>\nusername:<username>\npassword:<password>'");
                return null;
            }

            if (!credentialsArray[0].StartsWith("appid:"))
            {
                errors.Add(
                    @"Expected appid to be first segment but was not. Credentials should be in the format 'appid:<appid>\nusername:<username>\npassword:<password>'");
                return null;

            }
            if (!credentialsArray[1].StartsWith("username:"))
            {
                errors.Add(
                    @"Expected username to be second segment but was not. Credentials should be in the format 'appid:<appid>\nusername:<username>\npassword:<password>'");
                return null;

            }
            if (!credentialsArray[2].StartsWith("password:"))
            {
                errors.Add(
                    @"Expected password to be third segment but was not. Credentials should be in the format 'appid:<appid>\nusername:<username>\npassword:<password>'");
                return null;

            }
            credentialsArray[0] = credentialsArray[0].Replace("appid:", null);
            credentialsArray[1] = credentialsArray[1].Replace("username:", null);
            credentialsArray[2] = credentialsArray[2].Replace("password:", null);

            // there may be residual carriage returns (\r) in the data. Remove them if found.
            credentialsArray[0] = credentialsArray[0].Replace("\r", null);
            credentialsArray[1] = credentialsArray[1].Replace("\r", null);
            credentialsArray[2] = credentialsArray[2].Replace("\r", null);

            var user = CoreContext.Users.FirstOrDefault(p => p.Username.ToLower() == credentialsArray[1].ToLower());
            if (user == null)
            {
                errors.Add(
                    @$"User '{credentialsArray[1]}' was not found.");
                return null;
            }

            // Step 2: Verify that the app id exists and that the app contains the user; we don't want any user just using any app to access anything, only what that user is authorized to use
            var appAndUsername = CoreContext.Apps
                .Include(p => p.Users)
                .FirstOrDefault(
                    p => p.Id == credentialsArray[0] &&
                         p.Users.Any(u => u.Username == user.Username));


            if (appAndUsername != null)
            {
                // Step 3: Verify that the password given is correct
                if (IsPasswordCorrect(user.Id, credentialsArray[2]))
                {
                    // Step 4: Everything checks out fine? Generate a new token (or extend the existing one) and a new nOnce
                    if (string.IsNullOrEmpty(token)) return new Tuple<string, string>(GenerateUserToken(user.Id), GenerateNOnce(user.Id));

                    ExtendTokenDuration(token);
                    return new Tuple<string, string>(token, GenerateNOnce(user.Id));
                }
            }
            else
            {
                errors.Add("Either the app doesn't exist, the user is not authorized to use the app, or the given password is incorrect");
                return null;
            }
            // general error happened, return null
            errors.Add("An unexpected error occurred. Please make sure that all input parameters are correct");
            return null;
        }
        /// <summary>
        /// Extends the duration of the token by the value passed in to the minutesToAdd parameter
        /// </summary>
        /// <param name="tokenValue">The token's current value</param>
        /// <param name="minutesToAdd">The number of minutes to add to the token before expiration</param>
        public void ExtendTokenDuration(string tokenValue, int minutesToAdd = 10)
        {
            var tokenInBytes = Convert.FromBase64String(tokenValue);
            var token = CoreContext.UserTokens.FirstOrDefault(p => p.Value == tokenInBytes);
            if (token == null)
            {
                throw new Exception($"No token found with value '{tokenValue}'");
            }

            token.ExpiryDateUtc = DateTime.UtcNow.AddMinutes(minutesToAdd);
            token.Version = token.Version++;
            token.LastUpdatedUtc = DateTime.UtcNow;
            CoreContext.Update(token);
            CoreContext.SaveChanges();
        }
        /// <summary>
        /// Generates a new NOnce
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="minutesToAdd">The number of minutes from now before the expiration of the NOnce</param>
        /// <returns>The generated NOnce</returns>
        public string GenerateNOnce(string userId, int minutesToAdd = 10)
        {
            var nOnce = new NOnce
            {
                ExpiryDateUtc = DateTime.UtcNow.AddMinutes(minutesToAdd),
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                Version = 1,
                UserId = userId,
                Value = GenerateCryptographicRandomBytes()
            };
            CoreContext.NOnces.Add(nOnce);

            CoreContext.SaveChanges();
            return Convert.ToBase64String(nOnce.Value);
        }
        /// <summary>
        /// Generates a user token. This is different from the NOnce. The NOnce is only meant to be used ONCE (hence the name). The user token can be reused but its expiration must be refreshed
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="minutesToAdd">The number of minutes from now before the expiration of the token</param>
        /// <returns>The generated user token</returns>
        private string GenerateUserToken(string userId, int minutesToAdd = 10)
        {
            var userToken = new UserToken
            {
                ExpiryDateUtc = DateTime.UtcNow.AddMinutes(minutesToAdd),
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                Version = 1,
                UserId = userId,
                Value = GenerateCryptographicRandomBytes()
            };
            CoreContext.UserTokens.Add(userToken);
            CoreContext.SaveChanges();
            return Convert.ToBase64String(userToken.Value);
        }
        /// <summary>
        /// Generates a cryptographically-strong set of bytes
        /// </summary>
        /// <param name="size">The byte size</param>
        /// <returns>The cryptographic byte array</returns>
        public static byte[] GenerateCryptographicRandomBytes(int size = 64)
        {
            var cryptographicBytes = new byte[size];
            var csp = new RNGCryptoServiceProvider();
            csp.GetBytes(cryptographicBytes);
            return cryptographicBytes;

        }
    }
}
