namespace Erogemy.BlockBreaker.Model
{
    [System.Serializable]
    public class BockBreakerSettings
    {
        public int ballCount = 3;
        public int ballMoveSpeed = 5;
        public int paddleMoveSpeed = 10;
        public int skipPhaseThreshold = 3;
        public bool recoverBallOnPhaseClear = true; // phase進行時にボール数を復活
    }
}
