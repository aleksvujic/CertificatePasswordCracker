using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace CertificatePasswordCracker
{
    public class Program
    {
        private static readonly int MAX_PASSWORD_LENGTH = 15;
        private static readonly List<char> ALLOWED_CHARACTERS = new List<char>();
        
        public static void Main()
        {
            InitializeAllowedCharacters();

            // get all certificates
            string certificatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Certificates");
            Console.WriteLine($"Trying to find certificates in {certificatesPath}");
            string[] certificatePaths = Directory.GetFiles(certificatesPath);
            
            if (certificatePaths == null || certificatePaths.Length == 0)
            {
                Console.WriteLine("No certificates found, exiting...");
                Environment.Exit(1);
                return;
            }

            // crack each certificate password
            foreach (string certificatePath in certificatePaths)
            {
                CrackPassword(certificatePath);
            }
        }

        private static void InitializeAllowedCharacters()
        {
            for (char letter = 'a'; letter <= 'z'; letter++)
            {
                ALLOWED_CHARACTERS.Add(letter);
            }

            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                ALLOWED_CHARACTERS.Add(letter);
            }

            ALLOWED_CHARACTERS.Add('.');
            ALLOWED_CHARACTERS.Add('!');
            ALLOWED_CHARACTERS.Add('-');
            ALLOWED_CHARACTERS.Add('_');
        }

        private static void CrackPassword(string certificatePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string certificateName = Path.GetFileName(certificatePath);
            Console.WriteLine($"[{certificateName}] Cracking password");
            
            for (int length = 1; length <= MAX_PASSWORD_LENGTH; length++)
            {
                Console.WriteLine($"[{certificateName}] Trying passwords of length {length}");

                BigInteger numTries = 0;
                bool passwordFound = false;

                foreach (string password in CombinationsWithRepetition(ALLOWED_CHARACTERS, length))
                {
                    try
                    {
                        // try to open certificate with generated password
                        X509Certificate2 cert = new X509Certificate2(certificatePath, password);

                        // password was correct
                        passwordFound = true;
                        stopwatch.Stop();
                        Console.WriteLine($"[{certificateName}] Password is: {password}, found in {numTries} tries, elapsed time: {FormatTimeSpan(stopwatch.Elapsed)}");
                        break;
                    }
                    catch { }

                    // log a message every 1000 tries
                    if (numTries % 1000 == 0)
                    {
                        Console.WriteLine($"[{certificateName}] Number of tries for length {length} is {numTries}, elapsed time: {FormatTimeSpan(stopwatch.Elapsed)}");
                    }

                    numTries++;
                }

                if (passwordFound)
                {
                    break;
                }
            }
        }

        private static IEnumerable<string> CombinationsWithRepetition(IEnumerable<char> characters, int length)
        {
            if (length <= 0)
            {
                yield return string.Empty;
            }

            foreach (char character in characters)
            {
                foreach (string combination in CombinationsWithRepetition(characters, length-1))
                {
                    yield return $"{character}{combination}";
                }
            }
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        }
    }
}
