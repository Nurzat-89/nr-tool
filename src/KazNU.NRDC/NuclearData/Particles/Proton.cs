namespace NuclearData
{
    /// <summary>
    /// Model for proton
    /// </summary>
    internal class Proton : Particle
    {
        public Proton() : base(1, 1, Constants.MassOfProton)
        {
            Name = "Proton";
        }
    }
}
