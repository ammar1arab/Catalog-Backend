using Microsoft.AspNetCore.DataProtection;

namespace Backend.Helpers
{
    public class PasswordHelper(IDataProtectionProvider provider)
    {
        private readonly IDataProtector _protector = provider.CreateProtector("ConnectionStringsProtector");
        private readonly SimpleEncryptionHelper _simpleEncryptionHelper = new SimpleEncryptionHelper();

        public string Encrypt(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string Decrypt(string encryptedText)
        {
            return _protector.Unprotect(encryptedText);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string EncryptWithSimple(string plainText)
        {
            return _simpleEncryptionHelper.MapingStringForEncryption(plainText);
        }

        public string DecryptWithSimple(string encryptedText)
        {
            return _simpleEncryptionHelper.MapingStringForEncryption(encryptedText);
        }
    }
}
