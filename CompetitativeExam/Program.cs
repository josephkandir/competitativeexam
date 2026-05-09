using System.Text;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Custom Encoder/Decoder!\n");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Reverse Pair (e.g. HELLO is SVOOL and Hello is Svool)");
            Console.WriteLine("2. Forward-Backward (e.g. HELLO is JDNKQ and Hello is Jgnnq)");
            Console.WriteLine("3. Both #1 and #2 (e.g. HELLO is UUQNN and Hello is Uuqnn)");
            Console.WriteLine("\nSelect option (0/1/2/3):");
            var choice = Console.ReadLine()?.Trim();

            if (choice is not ("0" or "1" or "2" or "3"))
            {
                Console.WriteLine("Invalid option. Press any key to try again...");
                Console.ReadKey();
                continue;
            }

            if(choice == "0")
                break;

            Console.WriteLine("Enter text?");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                continue;

            string encoded = choice switch
            {
                "1" => CustomEncoder.EncodeReversePair(input),
                "2" => CustomEncoder.EncodeForwardBackward(input),
                _   => CustomEncoder.EncodeBoth(input)
            };
            Console.WriteLine("Encoded : " + encoded);

            string decoded = choice switch
            {
                "1" => CustomEncoder.DecodeReversePair(encoded),
                "2" => CustomEncoder.DecodeForwardBackward(encoded),
                _   => CustomEncoder.DecodeBoth(encoded)
            };
            Console.WriteLine("Decoded : " + decoded);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}

public class CustomEncoder
{
    /// <summary>
    /// Encodes the input string using the Reverse Pair cipher.
    /// Each letter is mapped to its mirror position in the alphabet.
    /// Non-letter characters are passed through unchanged.
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>The encoded string.</returns>
    public static string EncodeReversePair(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (char ch in input)
        {
            if (!char.IsLetter(ch)) { result.Append(ch); continue; }

            bool isUpper = char.IsUpper(ch);
            char start = isUpper ? 'A' : 'a';
            int pos = ch - start;          // 0-based position in alphabet
            char mapped = (char)(start + 25 - pos); // mirror: 0->25, 1->24, …
            result.Append(mapped);
        }

        return result.ToString();
    }

    /// <summary>
    /// Decodes a string that was encoded using the reverse pair encoding scheme.
    /// </summary>
    /// <param name="input">The encoded string to decode. Cannot be null.</param>
    /// <returns>A string representing the decoded value of the input.</returns>
    public static string DecodeReversePair(string input) => EncodeReversePair(input);

    /// <summary>
    /// Encodes the specified string by shifting each letter forward or backward based on its position in the string.
    /// </summary>
    /// <remarks>The encoding preserves the case of each letter. The method does not modify the original input
    /// string.</remarks>
    /// <param name="input">The input string to encode. Non-letter characters are not modified.</param>
    /// <returns>A new string in which each letter is shifted forward by two positions if its index is even, or backward by one
    /// position if its index is odd. Non-letter characters remain unchanged.</returns>
    public static string EncodeForwardBackward(string input)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (!char.IsLetter(ch)) { result.Append(ch); continue; }

            int shift = (i % 2 == 0) ? 2 : -1;
            result.Append(ShiftCharacter(ch, shift, char.IsUpper(ch)));
        }

        return result.ToString();
    }

    /// <summary>
    /// Decodes a string that was previously encoded using a forward-backward letter shifting scheme.
    /// </summary>
    /// <remarks>This method reverses a specific encoding where even-indexed letters were shifted forward by
    /// two and odd-indexed letters were shifted backward by one. Use this method only with strings encoded by the
    /// corresponding encoding algorithm.</remarks>
    /// <param name="input">The encoded string to decode. Non-letter characters are preserved as-is.</param>
    /// <returns>A decoded string with original letter characters restored. Non-letter characters remain unchanged.</returns>
    public static string DecodeForwardBackward(string input)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (!char.IsLetter(ch)) { result.Append(ch); continue; }

            int shift = (i % 2 == 0) ? -2 : 1;   // reverse of encode shifts
            result.Append(ShiftCharacter(ch, shift, char.IsUpper(ch)));
        }

        return result.ToString();
    }

    /// <summary>
    /// Encodes the specified string using both forward-backward and reverse pair encoding algorithms.
    /// </summary>
    /// <remarks>This method combines two encoding strategies to transform the input string. The exact
    /// encoding behavior depends on the implementations of the underlying encoding methods.</remarks>
    /// <param name="input">The string to encode. Cannot be null.</param>
    /// <returns>A string that represents the encoded result of applying both encoding algorithms to the input.</returns>
    public static string EncodeBoth(string input)
        => EncodeForwardBackward(EncodeReversePair(input));

    /// <summary>
    /// Decodes the specified input string by applying both forward-backward and reverse-pair decoding operations.
    /// </summary>
    /// <param name="input">The encoded string to decode. Cannot be null.</param>
    /// <returns>A string containing the fully decoded result. Returns an empty string if the input is empty.</returns>
    public static string DecodeBoth(string input)
        => DecodeReversePair(DecodeForwardBackward(input));

    /// <summary>
    /// Shifts the specified alphabetic character by a given number of positions within the alphabet, preserving its
    /// case.
    /// </summary>
    /// <remarks>The shift wraps around the alphabet. For example, shifting 'Z' by 1 returns 'A'.
    /// Non-alphabetic characters are not supported.</remarks>
    /// <param name="ch">The character to shift. Must be an uppercase or lowercase English letter.</param>
    /// <param name="shift">The number of positions to shift the character. Positive values shift forward; negative values shift backward.</param>
    /// <param name="isUpper">Indicates whether the character is uppercase. Set to <see langword="true"/> for uppercase letters; otherwise,
    /// <see langword="false"/>.</param>
    /// <returns>A character representing the shifted letter, maintaining the original case.</returns>
    private static char ShiftCharacter(char ch, int shift, bool isUpper)
    {
        char start = isUpper ? 'A' : 'a';
        int pos = (ch - start + shift + 26) % 26;
        return (char)(start + pos);
    }
}
