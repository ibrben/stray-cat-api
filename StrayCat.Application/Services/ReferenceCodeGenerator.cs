using System.Text;

namespace StrayCat.Application.Services
{
    public interface IReferenceCodeGenerator
    {
        string GenerateReferenceCode();
    }

    public class ReferenceCodeGenerator : IReferenceCodeGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string GenerateReferenceCode()
        {
            var result = new StringBuilder(7);
            
            for (int i = 0; i < 7; i++)
            {
                result.Append(_characters[_random.Next(_characters.Length)]);
            }
            
            return result.ToString();
        }
    }
}
