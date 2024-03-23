using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

// Klasa reprezentująca pojedynczy tweet
public class Tweet
{
    public string? Text { get; set; } 
    public string? UserName { get; set; } 
    public string? LinkToTweet { get; set; } 
    public string? FirstLinkUrl { get; set; } 
    public string? CreatedAt { get; set; } 
    public string? TweetEmbedCode { get; set; } 
}

// Klasa reprezentująca zestaw danych tweetów
public class Data
{
    public List<Tweet>? data { get; set; } 
}

class Program
{
    static void Main(string[] args)
    {
        // Wywołanie metody do odczytu pliku JSON
        var tweetsData = LoadTweetsFromJson("data.json");

        // Sortowanie tweetów po nazwie użytkowników
        SortTweetsByUserName(tweetsData);

        // Sortowanie użytkowników po dacie utworzenia tweeta
        SortUsersByTweetDate(tweetsData);
        // Zapis danych tweetów do pliku XML
        SaveTweetsToXml(tweetsData, "tweets.xml");

        // Obliczenie częstości występowania słów w treści tweetów
        // Obliczenie częstości występowania słów o długości co najmniej 5 liter w treści tweetów
        var wordFrequency = CalculateWordFrequency(tweetsData);

        // Wybierz 10 najczęściej występujących słów
        var topWords = wordFrequency.Where(pair => pair.Key.Length >= 5)
                                    .OrderByDescending(pair => pair.Value)
                                    .Take(10);

        // Wypisz 10 najczęściej występujących słów
        Console.WriteLine("10 najczęściej występujących w tweetach wyrazów o długości co najmniej 5 liter:");
        foreach (var pair in topWords)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value} razy");
        }
        Console.WriteLine();

        // Obliczenie IDF dla wszystkich słów w tweetach
        var idfValues = CalculateIDF(tweetsData);

        // Posortowanie IDF malejąco
        var sortedIDF = idfValues.OrderByDescending(pair => pair.Value);

        // Wypisz 10 wyrazów o największej wartości IDF
        Console.WriteLine("10 wyrazów o największej wartości IDF:");
        foreach (var pair in sortedIDF.Take(10))
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
        Console.WriteLine();
    }

    // Metoda do odczytu danych z pliku JSON
    static Data LoadTweetsFromJson(string filePath)
    {
        try
        {
            var jsonContent = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Data>(jsonContent);
            return data!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas wczytywania pliku JSON: {ex.Message}");
            return new Data();
        }
    }

    // Metoda do zapisu danych do pliku XML
    static void SaveTweetsToXml(Data tweetsData, string filePath)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(Data));
            using (var writer = XmlWriter.Create(filePath))
            {
                serializer.Serialize(writer, tweetsData);
            }
            Console.WriteLine("Dane tweetów zostały zapisane do pliku XML.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas zapisywania danych do pliku XML: {ex.Message}");
        }
    }
    // Metoda do sortowania tweetów po nazwie użytkownika
    static void SortTweetsByUserName(Data tweetsData)
    {
        if (tweetsData != null && tweetsData.data != null)
        {
            tweetsData.data = tweetsData.data.OrderBy(tweet => tweet.UserName).ToList();
            Console.WriteLine("Tweety zostały posortowane po nazwie użytkownika.\n");
        }
    }

    // Metoda do sortowania użytkowników po dacie utworzenia tweeta
    static void SortUsersByTweetDate(Data tweetsData)
    {
        if (tweetsData != null && tweetsData.data != null)
        {
            tweetsData.data = tweetsData.data.OrderBy(tweet => DateTime.Parse(tweet.CreatedAt.Replace(" at ", " "))).ToList();
            Console.WriteLine("Użytkownicy zostali posortowani po dacie utworzenia tweetu.\n");

            // Wypisanie informacji o najnowszym i najstarszym tweecie
            if (tweetsData.data.Count > 0)
            {
                var latestTweet = tweetsData.data.Last();
                var oldestTweet = tweetsData.data.First();
                Console.WriteLine("Najnowszy tweet:");
                PrintTweet(latestTweet);
                Console.WriteLine("Najstarszy tweet:");
                PrintTweet(oldestTweet);
            }
            else
            {
                Console.WriteLine("Brak tweetów do wypisania.");
            }
        }
        else
        {
            Console.WriteLine("Brak danych tweetów.");
        }
    }

    // Metoda do wydrukowania informacji o tweecie
    static void PrintTweet(Tweet tweet)
    {
        Console.WriteLine($"Tekst: {tweet.Text}");
        Console.WriteLine($"Nazwa użytkownika: {tweet.UserName}");
        Console.WriteLine($"Data utworzenia: {tweet.CreatedAt}");
        Console.WriteLine($"Link do tweeta: {tweet.LinkToTweet}");
        Console.WriteLine();
    }

    // Metoda do tworzenia słownika tweetów, gdzie kluczem jest nazwa użytkownika
    static Dictionary<string, List<Tweet>> CreateTweetDictionary(Data tweetsData)
    {
        Dictionary<string, List<Tweet>> tweetDictionary = new Dictionary<string, List<Tweet>>();

        if (tweetsData != null && tweetsData.data != null)
        {
            foreach (var tweet in tweetsData.data)
            {
                if (!tweetDictionary.ContainsKey(tweet.UserName))
                {
                    tweetDictionary[tweet.UserName] = new List<Tweet>();
                }
                tweetDictionary[tweet.UserName].Add(tweet);
            }
        }
        return tweetDictionary;
    }

    // Metoda do obliczania częstości występowania słów w tweetach
    static Dictionary<string, int> CalculateWordFrequency(Data tweetsData)
    {
        var wordFrequency = new Dictionary<string, int>();

        if (tweetsData != null && tweetsData.data != null)
        {
            foreach (var tweet in tweetsData.data)
            {
                // Podziel treść tweetu na słowa, używając spacji jako separatora
                var words = tweet.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Iteruj przez każde słowo i zaktualizuj częstość występowania
                foreach (var word in words)
                {
                    // Normalizuj słowo usuwając znaki interpunkcyjne i zmieniając na małe litery
                    var normalizedWord = word.Trim().Trim('.', ',', '!', '?').ToLower();

                    if (!string.IsNullOrEmpty(normalizedWord))
                    {
                        if (!wordFrequency.ContainsKey(normalizedWord))
                        {
                            wordFrequency[normalizedWord] = 1;
                        }
                        else
                        {
                            wordFrequency[normalizedWord]++;
                        }
                    }
                }
            }
        }
        return wordFrequency;
    }

    // Metoda do obliczania IDF dla słów w tweetach
    static Dictionary<string, double> CalculateIDF(Data tweetsData)
    {
        var idfValues = new Dictionary<string, double>();

        if (tweetsData != null && tweetsData.data != null)
        {
            var totalTweets = tweetsData.data.Count;
            // Obliczenie DF dla każdego słowa
            var documentFrequency = new Dictionary<string, int>();
            foreach (var tweet in tweetsData.data)
            {
                var wordsInTweet = new HashSet<string>(); // Używamy HashSet do uniknięcia podwójnego zliczania słów w jednym tweecie

                // Podziel treść tweetu na słowa, używając spacji jako separatora
                var words = tweet.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Zaktualizuj DF dla każdego słowa w danym tweecie
                foreach (var word in words)
                {
                    var normalizedWord = word.Trim().Trim('.', ',', '!', '?').ToLower();
                    if (!string.IsNullOrEmpty(normalizedWord) && normalizedWord.Length >= 5)
                    {
                        if (!wordsInTweet.Contains(normalizedWord))
                        {
                            wordsInTweet.Add(normalizedWord);
                            if (!documentFrequency.ContainsKey(normalizedWord))
                            {
                                documentFrequency[normalizedWord] = 1;
                            }
                            else
                            {
                                documentFrequency[normalizedWord]++;
                            }
                        }
                    }
                }
            }

            // Oblicz IDF dla każdego słowa
            foreach (var pair in documentFrequency)
            {
                var word = pair.Key;
                var df = pair.Value;
                var idf = Math.Log((double)totalTweets / (double)df);
                idfValues[word] = idf;
            }
        }
        return idfValues;
    }
}
