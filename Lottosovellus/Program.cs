namespace Lottosovellus
{
    internal class Program
    {
        const decimal CostPerRound = 1.00m;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine($"=== Lotto Application (Live stats, stop with 'X') ==={Environment.NewLine}");
            PrintMenu();
            int choice;

            choice = ReadInt("Choice (1 / 2): ", 1, 2);

            int[] lottoNumbers = GetLottoNumbers();
            Array.Sort(lottoNumbers);

            PrintChosenLottoNumbers(lottoNumbers);

            if (choice == 1)
            {
                int selectedRounds = ReadInt("How many rounds to play? (1 - 1,000,000): ", 1, 1000000);
                PlaySelectedRounds(lottoNumbers, selectedRounds);
            }
            else
            {
                PlayOnLoop(lottoNumbers);
            }


        }
        static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            string text = Console.ReadLine();
            int value;
            bool ok = int.TryParse(text, out value);

            while (!ok)
            {
                Console.WriteLine("Give an whole number.");
                Console.Write(prompt);
                text = Console.ReadLine();
                ok = int.TryParse(text, out value);
            }

            return value;

        }
        static int ReadInt(string prompt, int min, int max)
        {
            int value = ReadInt(prompt);

            while (value < min || value > max)
            {
                value = ReadInt(prompt);
            }

            return value;

        }
        static void PrintMenu()
        {
            Console.WriteLine("Select play mode:");
            Console.WriteLine("1) Fixed number of rounds (prints each round)");
            Console.WriteLine("2) Continuous auto-play (live stats: press 'X' to stop)");
        }
        static int[] GetLottoNumbers()
        {
            while (true)
            {
                Console.Write("Enter 7 unique numbers between 1-40 separated by ',': ");
                string input = Console.ReadLine();

                string[] parts = input.Split(',');

                bool valid;

                if (parts.Length != 7)
                {
                    Console.WriteLine($"> Input was incorrect, try again. <{Environment.NewLine}");
                    continue;
                }

                valid = true;
                List<int> numbers = new List<int>();

                foreach (string part in parts)
                {
                    if (int.TryParse(part, out int n))
                    {
                        if (n >= 1 && n <= 40)
                        {
                            numbers.Add(n);
                        }
                        else
                        {
                            valid = false;
                            break;
                        }
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid && numbers.Distinct().Count() != 7)
                {
                    Console.WriteLine("All numbers must be unique!");
                    continue;
                }
                if (valid)
                {
                    return numbers.ToArray();
                }

                Console.WriteLine("Enter whole numbers 1-40.");
            }
        }
        static void PrintChosenLottoNumbers(int[] numbers)
        {
            Console.WriteLine($"{Environment.NewLine}You chose numbers: [{numbers[0]}, {numbers[1]}, {numbers[2]}, {numbers[3]}, {numbers[4]}, {numbers[5]}, {numbers[6]}]{Environment.NewLine}");
        }
        static int[] WinningNumbersGenerator()
        {

            //Luodaan array joka on 40 indexin kokoinen.
            //Lisätään siihe for loopilla 1,2,3,4,5...40 saakka numerot.
            int[] numbers = new int[40];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i + 1;
            }

            //Käytetään Fisher-Yates shuffle algorytmiä voittavien numeroiden valitsemiseen.
            //Sekoitetaan tuo numbers[] randomilla ja valitaan sieltä sitten 7 ensimmäistä numeroa.
            //Tällä toteutuksella aina kaikki valitut numerot ovat uniikkeja.
            Random rand = new Random();

            int[] generatedLottoNumbers = new int[7];

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                int randomIndex = rand.Next(i, numbers.Length);

                int temp = numbers[i];
                numbers[i] = numbers[randomIndex];
                numbers[randomIndex] = temp;
            }
            Array.Copy(numbers, generatedLottoNumbers, 7);

            Array.Sort(generatedLottoNumbers);

            return generatedLottoNumbers;

        }
        static int CountNumberOfWins(int[] userNumbers, int[] winningNumbers)
        {
            int matches = 0;

            foreach (int number in userNumbers)
            {
                foreach (int win in winningNumbers)
                {
                    if (number == win)
                    {
                        matches++;
                        break;
                    }
                }
            }

            return matches;

        }
        static decimal DrawRound(int[] lottoNumbers)
        {
            int[] winningNumbers = WinningNumbersGenerator();

            int matches = CountNumberOfWins(lottoNumbers, winningNumbers);

            decimal winnings = Earnings(matches);

            return winnings;

        }
        static void PlaySelectedRounds(int[] lottoNumbers, int rounds)
        {
            int totalRoundsPlayed = 0;
            decimal totalWinnings = 0.00m;

            for (int i = 0; i < rounds; i++)
            {
                decimal roundWinnings = DrawRound(lottoNumbers);

                totalRoundsPlayed++;
                totalWinnings += roundWinnings;
            }
            PrintPlaySummary(totalRoundsPlayed, totalWinnings);
        }
        static void PlayOnLoop(int[] lottoNumbers)
        {
            int totalRoundsPlayed = 0;
            decimal totalWinnings = 0;

            Console.WriteLine($"---> Continious play started! Press 'X' to stop. <---{Environment.NewLine}");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.X)
                    {
                        break;
                    }
                }

                decimal roundWinnings = DrawRound(lottoNumbers);

                totalRoundsPlayed++;
                totalWinnings += roundWinnings;

                decimal totalMoneySpent = totalRoundsPlayed * CostPerRound;

                Console.Write($"\rRounds: {totalRoundsPlayed:N0} | Money spent: {totalMoneySpent:0.00} € | Winnings: {totalWinnings:0.00} € | ");

                Thread.Sleep(60);

            }
        }
        static decimal Earnings(int matches)
        {
            decimal winnings;

            switch (matches)
            {
                case 7:
                    winnings = 1000000.00m;
                    break;

                case 6:
                    winnings = 10000.00m;
                    break;

                case 5:
                    winnings = 100.00m;
                    break;

                case 4:
                    winnings = 10.00m;
                    break;

                case 3:
                    winnings = 1.00m;
                    break;

                default:
                    winnings = 0.00m;
                    break;
            }
            return winnings;
        }
        static void PrintPlaySummary(int roundsPlayed, decimal totalWinnings)
        {
            decimal moneySpent = roundsPlayed * CostPerRound;
            decimal netEarnings = totalWinnings - moneySpent;

            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}===> FINAL SUMMARY <===");
            Console.WriteLine($"Total rounds: {roundsPlayed}");
            Console.WriteLine($"Total spent: {moneySpent}");
            Console.WriteLine($"Total winnings: {totalWinnings}");
            Console.WriteLine($"Net result: {netEarnings}");
            Console.WriteLine($"{Environment.NewLine}Thanks for playing!");

        }
    }
}
