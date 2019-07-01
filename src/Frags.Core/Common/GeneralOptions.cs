public class GeneralOptions
{
    public char CommandPrefix { get; set; }

    public bool UseInMemoryDatabase { get; set; }
    public string DatabaseName { get; set; }

    public string AdminRole { get; set; }

    public int CharacterLimit { get; set; } = 25;
    public int EffectsLimit { get; set; } = 25;
}