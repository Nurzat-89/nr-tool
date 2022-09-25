namespace NuclearData
{
    /// <summary>
    /// Model for neutron
    /// </summary>
    internal class Neutron : Particle
    {
        public Neutron() : base(0 ,1, Constants.MassOfNeutron)
        {
            Name = "Neutron";
        }
    }
}
