# ATM-Machine

Welcome to the ATM Machine

please log in as one of the following users:

```
username: mlewellen
pin: 12345
```

```
username: jallen
pin: 54321
```

```
username: gsorensen
pin: 42914
```

Programming Steps:
1. Create a simple database: Create a comma separated text file, bank.txt. . Each line will have a user name, a pin #, and a current account balance for example:(you can make it more complicated with first and last name or whatever you'd like)
USERNAME	PIN	CURRENT BALANCE
mlewellen,	12345,	$45,000.00
jallen,	54321,	$15.00
gsorensen,	42914,	$140.00

2. Load the information from your database: Read the file into your program and split it into meaningful variables

3. Make your users login to the system: Collect a username and password and confirm a valid login. You can decide if they get three tries, or one failure kicks them out of the program

4. If they succeed display a menu: the options must include (but feel free to add a few more)
- Check Balance
- Withdraw
- Deposit
- Display last 5 transactions
- Quick Withdraw $40
- Quick Withdraw $100
- End current session

5. Write methods that simulate these option: You may add more but it should save at least these
Some logical methods would include: loadBankCustomers() saveBankCustomers() validateUser() checkBalance() withdrawMoney() depositMoney() quickWithdraw(int Amount) displayTranactions()

6. Tests should be written for user input

7. Tests should be written to confirm your arithmetic is right (What are the implications of depositing/withdrawing negative amounts?, you should probably test that your code doesn't allow that)

8. Update the database: When I come back tomorrow my balance should accurately reflect my previous days transactions. Make sure option 7 is updating the database with the new information
