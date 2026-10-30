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

            int gameMode;

            gameMode = ReadInt("Choice (1 / 2): ", 1, 2);

            // Kysytään käyttäjää syöttämään lottonumerot joilla pelataan.
            int[] lottoNumbers = GetLottoNumbers();
            Array.Sort(lottoNumbers);

            PrintChosenLottoNumbers(lottoNumbers);

            /*
            Valinta 1 = Käyttäjä syöttää kierrosten määrän joita pelataan.
            Valinta 2 = Jatkuva peli, jatkuvasti pelataan uusi kierros, kunnes pelaaja painaa 'X'.
            Tokaksi valinnaksi riittää else vaihtoehto, koska ReadInt()
            palauttaa vain 1 tai 2.
            */
            if (gameMode == 1)
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
            // Kokeillaan voiko syöte tekstin muuttaa numeroksi.
            Console.Write(prompt);
            string text = Console.ReadLine();

            int value; // Alustettu muuttuja johon käypä numero tallennettaan.

            bool ok = int.TryParse(text, out value);

            // Jos käännös ei onnistu syötettä kysytään uudelleen.
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
            // Tarkistetaan käyttäjän syötteen olevan halutulta väliltä
            int value = ReadInt(prompt);

            while (value < min || value > max)
            {
                Console.WriteLine($"{Environment.NewLine}You need to choose 1 or 2.");
                value = ReadInt(prompt);
            }

            return value;

        }
        static void PrintMenu()
        {
            Console.WriteLine("Select play mode:");
            Console.WriteLine("1) Fixed number of rounds");
            Console.WriteLine("2) Continuous auto-play (live stats: press 'X' to stop)");
        }
        static int[] GetLottoNumbers()
        {
            while (true)
            {
                Console.Write("Enter 7 unique numbers between 1-40 separated by ',': ");
                string input = Console.ReadLine();

                // Otetaan käyttäjän syöttämät numerot ja ne pilkotaan omiksi alkioiksi.
                string[] parts = input.Split(',');

                bool valid;

                // Tarkistetaan että käyttäjän syöte on tasan 7 alkiota.
                if (parts.Length != 7)
                {
                    Console.WriteLine($"> Input was incorrect, try again. <{Environment.NewLine}");
                    continue;
                }

                valid = true;

                /*
                Tehdään lista valmiiksi numeroille johonka sitten lisätään käyttäjän syöttämät kelvolliset numerot
                */
                List<int> numbers = new List<int>();

                foreach (string part in parts)
                {
                    if (int.TryParse(part, out int n)) // Tarkistetaan syötteen olevan kokonaisluku.
                    {
                        if (n >= 1 && n <= 40) // Tarkistetaan syötettyjen lukujen olevan 1-40.
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
                
                // Jos valid on true, tarkistetaan syötettyjen numeroiden olevan uniikkeja.
                if (valid && numbers.Distinct().Count() != 7) 
                {
                    Console.WriteLine("All numbers must be unique!");
                    continue;
                }
                if (valid) // Jos tarkistus menee läpi palautetaan numbers arrayn tiedot.
                {
                    return numbers.ToArray();
                }

                // Jos syötteiden tarkastukset eivät mene läpi tulostetaan virheilmoitus ja palataan while loopin alkuun.
                Console.WriteLine($"{Environment.NewLine}> Enter whole numbers only (1-40). <");
            }
        }
        static void PrintChosenLottoNumbers(int[] numbers)
        {
            Console.WriteLine($"{Environment.NewLine}You chose numbers: [ {string.Join(", ", numbers)} ]{Environment.NewLine}");
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
            Käytetään Fisher-Yates shuffle-algoritmia voittavien numeroiden valitsemiseen.
            Sekoitetaan numerot ja valitaan sieltä sitten 7 ensimmäistä numeroa.
            Tällä toteutuksella aina kaikki valitut numerot ovat uniikkeja, koska jokainen numero esiintyy vain kerran.
            */
            Random rand = new Random();

            int[] generatedLottoNumbers = new int[7]; // Tehdään valmiiksi array jossa on 7 indexiä.

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                int randomIndex = rand.Next(i, numbers.Length); // Generoidaa sattumalla indexin paikka rajatulta alueelta.

                int temp = numbers[i]; // Otetaan talteen alkuperäinen numero indexin paikalta.
                numbers[i] = numbers[randomIndex]; // Laitetaan arvottussa indexissä oleva arvo "i" indexin tilalle
                numbers[randomIndex] = temp; // Asetetaan alkuperäinen arvo arvotun indexin tilalle.
            }

            // Kopioidaan numbers arraysta seitsämän alkiota generatedLottoNumbersiin käyttämällä "7" parametriä
            // Kopiointi alkaa tässä tapauksessa vasemmalta oikealla ja indexin paikalta 0, koska ei ole syötetty aloitus indexiä.
            Array.Copy(numbers, generatedLottoNumbers, 7);

            Array.Sort(generatedLottoNumbers);

            return generatedLottoNumbers;

        }
        static int CountNumberOfWins(int[] userNumbers, int[] winningNumbers)
        {
            int matches = 0; // Alustetaan osumat.

            // Valitaan yks käyttäjän syöttämä numero.
            // Sisempi foreach silmukka, kun on ajettu valitaan seuraava käyttäjän numero.
            foreach (int number in userNumbers)
            {
                // Käydään kaikki voittavat numerot läpi ja verrataan onko jokin niistä sama, kun käyttäjällä
                // Jos jokin numeroista täsmää nostetaan laskinta ja lopetetaan sisempi foreach silmukan ajo.
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

            decimal winnings = Earnings(matches); //Palautetaan metodista voittosumma.

            return winnings;

        }
        static void PlaySelectedRounds(int[] lottoNumbers, int rounds)
        {
            // Alustetaan laskurit.
            int totalRoundsPlayed = 0; 
            decimal totalWinnings = 0.00m;

            // Ajetaan käyttäjän syöttämä määrä kierroksia.
            for (int i = 0; i < rounds; i++)
            {
                // Ajetaan kierros ja lisätään sen tulos totalWinnings muuttujaan.
                totalWinnings += DrawRound(lottoNumbers);

                // Nostetaan jokaisella kierroksella pelattujen kierrosten laskuria.
                totalRoundsPlayed++;
            }

            PrintPlaySummary(totalRoundsPlayed, totalWinnings);
        }
        static void PlayOnLoop(int[] lottoNumbers)
        {
            // Alustetaan laskurit.
            int totalRoundsPlayed = 0;
            decimal totalWinnings = 0;

            Console.WriteLine($"---> Continious play started! Press 'X' to stop. <---{Environment.NewLine}");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    // ReadKey(true) ei tulosta painettua näppäintä ruudulle.
                    ConsoleKey key = Console.ReadKey(true).Key;

                    // Lopettaa silmukan ainoastaan, jos 'X' näppäintä painetaan.
                    if (key == ConsoleKey.X)
                    {
                        break;
                    }
                }

                // Ajetaan kierros ja lisätään sen tulokset suoraan kokonaisvoittoihin.
                totalWinnings += DrawRound(lottoNumbers);

                // Nostetaan jokaisella kierroksella laskureita loppulaskentaa varten.
                totalRoundsPlayed++;

                // Nostetaan live datan kokonais käytetyn rahan määrää.
                decimal totalMoneySpent = totalRoundsPlayed * CostPerRound;

                Console.Write($"\r| Rounds: {totalRoundsPlayed:N0} | Money spent: {totalMoneySpent:0.00} € | Winnings: {totalWinnings:0.00} € | ");

                Thread.Sleep(60);

            }

            PrintPlaySummary(totalRoundsPlayed, totalWinnings);
        }
        static decimal Earnings(int matches)
        {
            // Tarkistetaan listasta matches avaimella mikä voitto annetaan.
            Dictionary<int, decimal> prizes = new Dictionary<int, decimal>
            {
                { 3, 1.00m },
                { 4, 10.00m },
                { 5, 100.00m },
                { 6, 10000.00m },
                { 7, 1000000.00m }
            };

            if (prizes.ContainsKey(matches))
            {
                return prizes[matches];
            }

            // Palautetaan 0 voitto jos matches arvon avain ei löydy dictionarystä.
            return 0.00m;

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
