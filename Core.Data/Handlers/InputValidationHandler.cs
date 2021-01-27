using System;
using System.Net.Mail;
using Core.Data.Contexts;

namespace Core.Data.Handlers
{
    public static class InputValidationHandler
    {
        /// <summary>
        /// Validates the email address based on the given expression.
        /// </summary>
        /// <param name="email">The email address to be validated</param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                // if this succeeds, it's valid. If it throws an error, it's not valid
                var emailAddress = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check whether or not the nominated password is "strong" enough
        /// </summary>
        /// <param name="password">The nominated password</param>
        /// <param name="context">The db context</param>
        /// <returns>True if the password is determined to be strong enough; otherweise, returns false.</returns>
        public static bool IsValidPassword(string password, CoreContext context)
        {
            //check for password length - 8 characters minimum
            return password.Length >= 8;

            //TODO: Add more validations if required
        }

    }
}
