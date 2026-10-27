namespace Lottosovellus
{
    internal class Program
    {
        const decimal CostPerRound = 1.00m;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine($"=== Lotto Application ==={Environment.NewLine}");
            PrintMenu();

            int choice;

            //Kysytään pelimuodon valinta.
            choice = ReadInt("Choice (1 / 2): ", 1, 2);

            //Kysytään käyttäjää syöttämään lottonumerot joilla pelataan.
            int[] lottoNumbers = GetLottoNumbers();
            Array.Sort(lottoNumbers);

            //Tulostetaan konsoliin valitut lottonumerot.
            PrintChosenLottoNumbers(lottoNumbers);

            /*
            Valinta 1 = Käyttäjä syöttää kierrosten määrän joita pelataan.
            Valinta 2 = Jatkuva peli, jatkuvasti pelataan uusi kierros, kunnes pelaaja painaa 'X'.
            Tokaksi valinnaksi riittää else vaihtoehto, koska ReadInt()
            palauttaa vain 1 tai 2.
            */
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

                //Otetaan käyttäjän syöttämät numerot ja ne pilkotaan omiksi alkioiksi.
                string[] parts = input.Split(',');

                //Alustetaan bool valid ennen silmukoita.
                bool valid;

                //Tarkistetaan että käyttäjän syöte on tasan 7 alkiota, jos ei ole aloitaan while loop alusta.
                if (parts.Length != 7)
                {
                    Console.WriteLine($"> Input was incorrect, try again. <{Environment.NewLine}");
                    continue;
                }

                valid = true;
                /*
                Tehdään lista valmiiksi numeroille johonka sitten lisätään käyttäjän syöttämät numerot
                ,jos ne ovat 1-40 ja uniikkeja.
                */
                List<int> numbers = new List<int>();

                foreach (string part in parts)
                {
                    if (int.TryParse(part, out int n)) //Tarkistetaan syötteen olevan kokonaisluku.
                    {
                        if (n >= 1 && n <= 40) //Tarkistetaan syötettyjen lukujen olevan 1-40.
                        {
                            numbers.Add(n);
                        }
                        else 
                        {
                            //Jos tarkistus ei mene läpi, lopetetaan foreach silmukka.
                            valid = false;
                            break;
                        }
                    }
                    else 
                    {
                        //Jos syöte ei ollut kokonaisluku lopetetaan foreach silmukan ajaminen.
                        valid = false;
                        break;
                    }
                }
                
                //Jos valid on true, tarkistetaan 7 numeron olevan uniikkeja.
                if (valid && numbers.Distinct().Count() != 7) 
                {
                    Console.WriteLine("All numbers must be unique!");
                    continue;
                }
                if (valid) //Jos tarkistus menee läpi palautetaan numbers taulokon tiedot.
                {
                    return numbers.ToArray();
                }

                //Jos syötteiden tarkastukset eivät mene läpi tulostetaan virheilmoitus ja palataan while loopin alkuun.
                Console.WriteLine($"{Environment.NewLine}> Enter whole numbers only (1-40). <");
            }
        }
        static void PrintChosenLottoNumbers(int[] numbers)
        {
            //Löytyisi varmasti järkevämpi vaihtoehto tälle....
            Console.WriteLine($"{Environment.NewLine}You chose numbers: [{numbers[0]}, {numbers[1]}, {numbers[2]}, {numbers[3]}, {numbers[4]}, {numbers[5]}, {numbers[6]}]{Environment.NewLine}");
        }
        static int[] WinningNumbersGenerator()
        {

            //Luodaan array joka on 40 alkion kokoinen.
            //Lisätään siihe for loopilla 1,2,3,4,5...40 saakka numerot.
            int[] numbers = new int[40];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i + 1;
            }

            /*
            Käytetään Fisher-Yates shuffle algorytmiä voittavien numeroiden valitsemiseen.
            Sekoitetaan tuo numbers[] randomilla ja valitaan sieltä sitten 7 ensimmäistä numeroa.
            Tällä toteutuksella aina kaikki valitut numerot ovat uniikkeja, koska jokainen numero esiintyy vain kerran.
            */
            Random rand = new Random();

            int[] generatedLottoNumbers = new int[7];

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                int randomIndex = rand.Next(i, numbers.Length);

                int temp = numbers[i];
                numbers[i] = numbers[randomIndex];
                numbers[randomIndex] = temp;
            }

            Array.Copy(numbers, generatedLottoNumbers, 7); //Kopioidaan numbers:in 7 ensimmäistä numeroa taulukkoon generatedLottoNumbers.

            Array.Sort(generatedLottoNumbers);

            return generatedLottoNumbers;

        }
        static int CountNumberOfWins(int[] userNumbers, int[] winningNumbers)
        {
            int matches = 0; //Alustetaan osumat.

            /*
            Valitaan yks käyttäjän lottonumero ja
            verrataan sitä jokaiseen generoituu voittavaan numeroon.
            Jos löytyy osuma nostetaan int matches yhdellä ylös ja lopetetaan sisempi foreach silmukan ajaminen.
            */

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

            int[] winningNumbers = WinningNumbersGenerator(); //Otetaan metodista voittavat numerot alkioon.

            int matches = CountNumberOfWins(lottoNumbers, winningNumbers); //Lasketaan voitto osumien määrä.

            decimal winnings = Earnings(matches); //Palautetaan metodista voitto summa.

            return winnings;

        }
        static void PlaySelectedRounds(int[] lottoNumbers, int rounds)
        {
            //Alustetaan laskurit.
            int totalRoundsPlayed = 0; 
            decimal totalWinnings = 0.00m;

            //Ajetaan käyttäjän syöttämä määrä kierroksia.
            for (int i = 0; i < rounds; i++)
            {
                //Ajaa kierroksen.
                decimal roundWinnings = DrawRound(lottoNumbers);

                //Nostetaan jokaisella kierroksella laskureita loppulaskentaa varten.
                totalRoundsPlayed++;
                totalWinnings += roundWinnings;
            }
            //Tulostetaan pelauksen yhteenveto.
            PrintPlaySummary(totalRoundsPlayed, totalWinnings);
        }
        static void PlayOnLoop(int[] lottoNumbers)
        {
            //Alustetaan laskurit.
            int totalRoundsPlayed = 0;
            decimal totalWinnings = 0;

            Console.WriteLine($"---> Continious play started! Press 'X' to stop. <---{Environment.NewLine}");

            //While silmukka pyörii kunnes käyttäjä painaa 'X' näppäintä.
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

                //Ajaa kierroksen.
                decimal roundWinnings = DrawRound(lottoNumbers);

                //Nostetaan jokaisella kierroksella laskureita loppulaskentaa varten.
                totalRoundsPlayed++;
                totalWinnings += roundWinnings;

                //Nostetaan live datan kokonais käytetyn rahan määrää.
                decimal totalMoneySpent = totalRoundsPlayed * CostPerRound;

                Console.Write($"\r| Rounds: {totalRoundsPlayed:N0} | Money spent: {totalMoneySpent:0.00} € | Winnings: {totalWinnings:0.00} € | ");

                Thread.Sleep(60);

            }
            //Tulostetaan pelauksen yhteenveto.
            PrintPlaySummary(totalRoundsPlayed, totalWinnings);
        }
        static decimal Earnings(int matches)
        {
            //Alustetaan voitot laskuri
            decimal winnings;

            //Osumä määrän perusteella annetaan voitot, alle 3 = 0 € voitto.
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

            decimal moneySpent = roundsPlayed * CostPerRound; //Lasketaan käytetty raha pelatuista kierroksista.
            decimal netEarnings = totalWinnings - moneySpent; //Lasketaan netto voitot.

            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}===> FINAL SUMMARY <===");
            Console.WriteLine($"Total rounds: {roundsPlayed}");
            Console.WriteLine($"Total spent: {moneySpent}");
            Console.WriteLine($"Total winnings: {totalWinnings}");
            Console.WriteLine($"Net result: {netEarnings}");
            Console.WriteLine($"{Environment.NewLine}Thanks for playing!");

        }
    }
}
