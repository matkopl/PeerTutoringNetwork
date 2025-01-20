using Xunit;
using PeerTutoringNetwork.Security;

namespace Test.AntonioTest.Security
{
    public class PasswordHashProviderTests
    {
        [Fact]
        public void GetSalt_ShouldReturnValidBase64Salt()
        {
            // Act
            var salt = PasswordHashProvider.GetSalt();

            // Assert
            Assert.False(string.IsNullOrEmpty(salt)); 
            Assert.True(IsBase64String(salt)); 
        }

        [Fact]
        public void GetHash_ShouldReturnValidHash_ForSamePasswordAndSalt()
        {
            // Arrange
            var password = "testPassword";
            var salt = PasswordHashProvider.GetSalt();

            // Act
            var hash = PasswordHashProvider.GetHash(password, salt);

            // Assert
            Assert.False(string.IsNullOrEmpty(hash));
            Assert.True(IsBase64String(hash)); 
        }

        [Fact]
        public void GetHash_ShouldReturnDifferentHash_ForDifferentSalt()
        {
            // Arrange
            var password = "testPassword";
            var salt1 = PasswordHashProvider.GetSalt();
            var salt2 = PasswordHashProvider.GetSalt();

            // Act
            var hash1 = PasswordHashProvider.GetHash(password, salt1);
            var hash2 = PasswordHashProvider.GetHash(password, salt2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        private bool IsBase64String(string str)
        {
            
            try
            {
                Convert.FromBase64String(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
