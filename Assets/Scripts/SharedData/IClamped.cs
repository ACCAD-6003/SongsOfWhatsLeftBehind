namespace SharedData
{
    public interface IClamped<in T>
    {
        public float MaxValue { get; }
        public float MinValue { get; }
        
        public void SetMax(T newMax);
        public void SetMin(T newMin);
    }
}