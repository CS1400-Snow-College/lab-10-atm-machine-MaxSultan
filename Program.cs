// Max Sultan, Nov 14th, 2026, Lab 10: ATM
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Globalization;

static string[] LoadFileSafe(string filePath){
    string[] result = [];
    if(File.Exists(filePath))
        result = File.ReadAllLines(filePath);
    return result;
}

static string[] LoadBankCustomers()
{
    return LoadFileSafe("./bank.txt");
} 

static string[] LoadBankTransactions()
{
    return LoadFileSafe("./transactions.txt");
} 

static (string id, string username) ValidateUser(string[] users)
{
    Console.WriteLine("Enter your username: ");
    string inputUsername = Console.ReadLine();

    Console.WriteLine("Enter your pin: ");
    string inputPin = Console.ReadLine();

    foreach(string serializedUser in users) {
        List<string> user = serializedUser.Split(',').ToList();

        (string id, string username, string salt, string hash) tmpUser = (user[0], user[1], user[2], user[3]);
        if (inputUsername == tmpUser.username && ComputeSha256Hash(inputPin + tmpUser.salt) == tmpUser.hash) 
            return (tmpUser.id, tmpUser.username);
    }
    Console.WriteLine("Error Logging in, Goodbye");
    return ("","");
}

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

static void WriteNewTransaction(string userId, decimal amount, DateTime time){
    List<string> serializedTransactions = LoadBankTransactions().ToList();
    serializedTransactions.Add($"{userId},{amount},{time.ToString("yyyy-MM-dd HH:mm:ss")}");
    File.WriteAllLines("transactions.txt", serializedTransactions);
}

static void Withdraw(int amount, string userId)
{
    if(amount > 0) {
        Console.WriteLine("Withdraw amounts should be negative");
        return;
    }
    if(CheckBalance(userId) < Math.Abs(amount)) {
        Console.WriteLine("Withdraw Amount was larger than available balance");
        return;
    }
    WriteNewTransaction(userId, (decimal)amount, DateTime.Now);
    Console.WriteLine($"Successfully withdrawn ${Math.Abs(amount)}");
}

static void Deposit(int amount, string userId){
    // do we need to check if this number is bigger than the largest number we can handle? What do we do in that case?
    if(amount < 0) {
        Console.WriteLine("Deposit amounts should be positive");
        return;
    }
    WriteNewTransaction(userId, (decimal)amount, DateTime.Now);
    Console.WriteLine($"Successfully deposited ${Math.Abs(amount)}");
}

static List<(string id, decimal amount, DateTime time)> LastFiveTransactions(string id)
{
    List<string> serializedTransactions = LoadBankTransactions().ToList();
    serializedTransactions = serializedTransactions.GetRange(1, serializedTransactions.Count - 1).ToList();
    List<(string id, decimal amount, DateTime time)> transactions = serializedTransactions.Select(transaction => {
        string[] values = transaction.Split(",");
        string[] acceptedFormats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd H:mm:ss" };
        if (!DateTime.TryParseExact(values[2], acceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
        {
            parsedTime = DateTime.Parse(values[2], CultureInfo.InvariantCulture);
        }
        return (values[0], decimal.Parse(values[1]), parsedTime);
    }).ToList();
    // sort by timestamp descending so the most recent events are first
    transactions = transactions.OrderByDescending(t => t.time).ToList();
    return transactions.Take(5).ToList();
}

static decimal CheckBalance(string id)
{
    List<string> serializedTransactions = LoadBankTransactions().ToList();
    return serializedTransactions.Aggregate(0.00m, (accumulator, currentValue) => {
        string[] values = currentValue.Split(",");
        return values[0] == id ? accumulator + decimal.Parse(values[1]) : accumulator;
    });

}

static void Main() 
{
    Console.Clear();
    string[] users = LoadBankCustomers();
    (string id, string username) currentUser = ValidateUser(users);
    
    Console.Clear();

    if(currentUser.id != ""){
        while(true)
        {

        Console.WriteLine($"Logged in as:{currentUser.username}");
        Console.WriteLine(@"
1) Check Balance
2) Withdraw
3) Deposit
4) Display last 5 transactions
5) Quick Withdraw $40
6) Quick Withdraw $100
7) End current session

        ");
        ConsoleKey inputKey = Console.ReadKey().Key;
        Console.WriteLine();
        if(inputKey == ConsoleKey.D7 || inputKey == ConsoleKey.NumPad7)
            break;
        else if (inputKey == ConsoleKey.D6 || inputKey == ConsoleKey.NumPad6)
            Withdraw(-100, currentUser.id);
        else if (inputKey == ConsoleKey.D5 || inputKey == ConsoleKey.NumPad5)
            Withdraw(-40, currentUser.id);
        else if (inputKey == ConsoleKey.D4 || inputKey == ConsoleKey.NumPad4)
        {
            Console.WriteLine($"{string.Format("{0,8}", "TYPE")} {string.Format("{0,8}", "AMOUNT")} {string.Format("{0,22}", "TIME")}");
            foreach((string id, decimal amount, DateTime time) transaction in LastFiveTransactions(currentUser.id))
            {
                string transactionType = transaction.amount > 0.00m ? "Deposit" : "Withdraw";       
                Console.WriteLine($"{transactionType}  {transaction.amount.ToString("C")}  {transaction.time}");
            }
        }
        else if (inputKey == ConsoleKey.D3 || inputKey == ConsoleKey.NumPad3)
        {
            Console.Write("Enter the amount to deposit: ");
            string inputAmount = Console.ReadLine();
            if(int.TryParse(inputAmount, out int parsedInputAmount)){
                if (parsedInputAmount < 0){
                    Console.WriteLine("There was an error with the Deposit, please try again");
                    return;
                }
                Deposit(parsedInputAmount, currentUser.id);
            } else {
                Console.WriteLine("There was an error with the Deposit, please try again");
            }
        }
        else if (inputKey == ConsoleKey.D2 || inputKey == ConsoleKey.NumPad2)
        {
            Console.Write("Enter the amount to deposit: ");
            string inputAmount = Console.ReadLine();
            if(int.TryParse(inputAmount, out int parsedInputAmount)){
                Withdraw(parsedInputAmount < 0 ? parsedInputAmount : (parsedInputAmount * -1), currentUser.id);
            } else {
                Console.WriteLine("There was an error with the Withdraw, please try again");
            }
        }
        else if (inputKey == ConsoleKey.D1 || inputKey == ConsoleKey.NumPad1)
            Console.WriteLine($"Your balance is: {CheckBalance(currentUser.id).ToString("C")}");
    }
    } else {
        Console.WriteLine("There was an error logging in");
    }

}

Main();