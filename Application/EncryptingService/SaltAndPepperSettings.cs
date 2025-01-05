namespace Application.EncryptingService
{
    public class SaltAndPepperSettings
    {
        public string PepperLetters { get; set; } = string.Empty;
        public string SaltLetters { get; set; } = string.Empty;
        public int PepperLength { get; set; }
        public int SaltLettersLength { get; set; }
    }
}