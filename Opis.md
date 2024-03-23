## Opis programu do analizy tweetów

Ten program został stworzony w języku C# i służy do analizy danych tweetów z pliku JSON. Poniżej znajdziesz opis głównych funkcji i operacji, które wykonuje.

### Struktury danych

1. **Tweet**: Klasa reprezentująca pojedynczy tweet, zawiera pola takie jak tekst tweeta, nazwa użytkownika, link do tweeta, data utworzenia itp.
2. **Data**: Klasa reprezentująca zestaw danych tweetów, zawiera listę obiektów typu Tweet.

### Główne funkcje programu

1. **Wczytywanie danych z pliku JSON**: Metoda `LoadTweetsFromJson` wczytuje dane z pliku JSON i deserializuje je do obiektu typu `Data`.

2. **Sortowanie tweetów po nazwie użytkownika**: Metoda `SortTweetsByUserName` sortuje listę tweetów alfabetycznie po nazwie użytkownika.

3. **Sortowanie użytkowników po dacie utworzenia tweeta**: Metoda `SortUsersByTweetDate` sortuje listę użytkowników według daty utworzenia ich ostatniego tweeta.

4. **Zapis danych do pliku XML**: Metoda `SaveTweetsToXml` zapisuje dane tweetów do pliku XML.

5. **Obliczanie częstości występowania słów w tweetach**: Metoda `CalculateWordFrequency` oblicza częstość występowania każdego słowa w tekście tweetów.

6. **Obliczanie IDF dla słów w tweetach**: Metoda `CalculateIDF` oblicza wartość IDF (Inverse Document Frequency) dla każdego słowa w tekście tweetów.

### Wyświetlanie wyników

Po przetworzeniu danych, program wyświetla:

- 10 najczęściej występujących słów o długości co najmniej 5 liter.
- 10 wyrazów o największej wartości IDF.

### Obsługa błędów

Program zawiera obsługę błędów podczas wczytywania danych z pliku JSON oraz podczas zapisywania danych do pliku XML.

## Autor

Lucjan Bator
