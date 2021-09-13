using TMPro;

namespace DefaultNamespace
{
    public interface IInputOutputProvider
    {
        TMP_InputField InputField { get; }
        TMP_InputField OutputField { get; }
    }
}