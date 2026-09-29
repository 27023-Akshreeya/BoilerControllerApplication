namespace MusicalScalePlayer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var notes = new Notes();
            while (true)
            {
                Console.Write("Add a new seqence\nEnter the number of notes you want in the sequence:");
                if(!int.TryParse(Console.ReadLine(), out int value))
                {
                    Console.WriteLine("Invalid input");
                    break;
                }
                Console.WriteLine("Available notes:");
                foreach (var note in notes.StandardNotes)
                {
                    Console.Write($"{note.Name} ");
                }
                Console.Write("\nEnter note sequence:");
                var newSequence = new List<int>();
                for (int i = 0; i < value ; i++)
                {
                    string addnote = Console.ReadLine() ?? string.Empty;
                    int noteFrequency = notes.StandardNotes.First(n => n.Name.Equals(addnote, StringComparison.OrdinalIgnoreCase)).Frequency;
                    newSequence.Add(noteFrequency);
                }
                notes.sequences.Add(newSequence);
                Console.WriteLine("Enter the speed of the note you want to play (in ms):");
                if (!int.TryParse(Console.ReadLine(), out int speed))
                {
                    Console.WriteLine("Invalid input");
                    break;
                }
                foreach (var sequence in newSequence)
                {
                    Console.Beep(sequence, speed);
                }
                Console.WriteLine("Do you want to add new sequence? [y/n]");
                string choice = Console.ReadLine() ?? "n";
                if (choice.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }
    }
    public class MusicalNote
    {
        public int Frequency { get; set; }
        public string? Name { get; set; }

        public MusicalNote(int frequency, string name)
        {
            this.Frequency = frequency;
            this.Name = name;
        }
    }

    public class Notes
    {
        public List<MusicalNote> StandardNotes = new List<MusicalNote>
        {
            new MusicalNote(261, "C"),
            new MusicalNote(277, "C#"),
            new MusicalNote(293, "D"),
            new MusicalNote(311, "D#"),
            new MusicalNote(329, "E"),
            new MusicalNote(349, "F"),
            new MusicalNote(369, "F#"),
            new MusicalNote(392, "G"),
            new MusicalNote(415, "G#"),
            new MusicalNote(440, "A"),
            new MusicalNote(466, "A#"),
            new MusicalNote(493, "B"),
        };

        public List<List<int>> sequences = new List<List<int>>();
    }
}
