// Max Sultan, Nov 14th, 2026, Lab 10: ATM
using System.Text;
using System.Security.Cryptography;

static byte[] GenerateSalt(int byteLength)
{
    byte[] salt = new byte[byteLength];
    RandomNumberGenerator.Create().GetBytes(salt);
    return salt;
}


static string ComputeSha256Hash(string rawData)
{
    byte[] bytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rawData));
    StringBuilder builder = new StringBuilder();
    for (int i = 0; i < bytes.Length; i++)
        builder.Append(bytes[i].ToString("x2")); // "x2" formats as two lowercase hexadecimal digits
    return builder.ToString();
}

static void Main() 
{
    Console.Clear();
    Console.WriteLine("Enter your username: ");
    string inputUsername = Console.ReadLine();

    Console.WriteLine("Enter your pin: ");
    string inputPin = Console.ReadLine();

    string[] users = File.ReadAllLines("./bank.txt");
    foreach(string serializedUser in users) {
        List<string> user = serializedUser.Split(',').ToList();

        (string id, string username, string salt, string hash) tmpUser = (user[0], user[1], user[2], user[3]);
        Console.WriteLine(tmpUser.username);
        if (inputUsername == tmpUser.username) 
        {
            Console.WriteLine(ComputeSha256Hash(inputPin + tmpUser.salt));
            if(ComputeSha256Hash(inputPin + tmpUser.salt) == tmpUser.hash){
                Console.WriteLine($"Logged in {tmpUser.username}");

            } else {
                Console.WriteLine($"{ComputeSha256Hash(inputPin + tmpUser.salt)} did not equal {tmpUser.hash}");
            }
        } 
    }

}

Main();