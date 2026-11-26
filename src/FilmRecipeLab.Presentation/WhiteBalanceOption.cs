namespace FilmRecipeLab.Presentation;

public sealed record WhiteBalanceOption(string Code, string Label)
{
    public override string ToString() => Label;
}
