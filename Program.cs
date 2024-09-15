using System;
using System.IO;
using System.Text;

class ByteToDnaConverter
{
    // Method to convert bytes to DNA sequence
    public static string ByteToDna(byte[] bytes)
    {
        string[] dnaBases = { "A", "C", "T", "G" };
        StringBuilder dnaSequence = new StringBuilder();

        foreach (byte b in bytes)
        {
            // Convert each byte into a DNA sequence of 4 bases
            for (int i = 6; i >= 0; i -= 2)
            {
                int twoBits = (b >> i) & 0b11; // Extract 2 bits
                dnaSequence.Append(dnaBases[twoBits]); // Append DNA base
            }
        }

        return dnaSequence.ToString();
    }

    // Method to convert a file extension into a DNA sequence
    public static string ExtensionToDna(string extension)
    {
        // Ensure the extension is 4 characters long
        if (extension.Length < 4)
        {
            extension = extension.PadRight(4, '\0'); // Pad with null characters if less than 4
        }

        byte[] extensionBytes = Encoding.ASCII.GetBytes(extension);
        return ByteToDna(extensionBytes);
    }

    // Method to convert a DNA sequence back to bytes
    public static byte[] DnaToBytes(string dna)
    {
        byte[] bytes = new byte[dna.Length / 4];

        for (int i = 0; i < dna.Length; i += 4)
        {
            byte b = 0;

            // Convert 4 DNA bases back to 8 bits (1 byte)
            for (int j = 0; j < 4; j++)
            {
                b <<= 2; // Shift left by 2 bits
                char baseChar = dna[i + j];

                switch (baseChar)
                {
                    case 'A': b |= 0b00; break;
                    case 'C': b |= 0b01; break;
                    case 'T': b |= 0b10; break;
                    case 'G': b |= 0b11; break;
                }
            }

            bytes[i / 4] = b;
        }

        return bytes;
    }

    // Method to convert a file to DNA format and save it
    public static void ConvertFileToDna(string filePath)
    {
        // Read the file's bytes
        byte[] fileBytes = File.ReadAllBytes(filePath);

        // Get the file extension (e.g., "txt")
        string extension = Path.GetExtension(filePath).TrimStart('.');

        // Convert the extension to a DNA sequence
        string dnaExtension = ExtensionToDna(extension);

        // Convert the file bytes to a DNA sequence
        string dnaFileData = ByteToDna(fileBytes);

        // Combine the extension and file DNA data
        string fullDnaSequence = dnaExtension + dnaFileData;

        // Get the file name without extension
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        // Set the new file path with the ".dna" extension
        string dnaFilePath = Path.Combine(Path.GetDirectoryName(filePath), fileNameWithoutExtension + ".dna");

        // Write the DNA sequence to the new file
        File.WriteAllText(dnaFilePath, fullDnaSequence);

        Console.WriteLine($"File converted to DNA format and saved as: {dnaFilePath}");
    }

    // Method to convert a DNA file back to its original format
    public static void ConvertDnaToFile(string dnaFilePath)
    {
        // Read the DNA sequence from the file
        string dnaSequence = File.ReadAllText(dnaFilePath);

        // Extract the first 16 characters (4 bytes) for the file extension
        string dnaExtension = dnaSequence.Substring(0, 16); // First 16 characters for 4-byte extension

        // Convert DNA extension back to bytes
        byte[] extensionBytes = DnaToBytes(dnaExtension);
        string fileExtension = Encoding.ASCII.GetString(extensionBytes).TrimEnd('\0'); // Remove any null characters

        // Convert the rest of the DNA sequence back to the original file bytes
        string dnaFileData = dnaSequence.Substring(16); // The rest of the DNA sequence
        byte[] fileBytes = DnaToBytes(dnaFileData);

        // Set the original file path with the restored extension
        string originalFileName = Path.GetFileNameWithoutExtension(dnaFilePath);
        string restoredFilePath = Path.Combine(Path.GetDirectoryName(dnaFilePath), originalFileName + "." + fileExtension);

        // Write the original file bytes back to the restored file
        File.WriteAllBytes(restoredFilePath, fileBytes);

        Console.WriteLine($"DNA file converted back to original format and saved as: {restoredFilePath}");
    }

    // Main method
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide the path to the file.");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine("The specified file does not exist.");
            return;
        }

        if (Path.GetExtension(filePath).Equals(".dna", StringComparison.OrdinalIgnoreCase))
        {
            // Convert from DNA format back to original file
            ConvertDnaToFile(filePath);
        }
        else
        {
            // Convert file to DNA format
            ConvertFileToDna(filePath);
        }
    }
}
