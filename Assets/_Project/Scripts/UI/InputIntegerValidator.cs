using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class InputIntegerValidator : MonoBehaviour
{
    // Allowed sign Modes
    public enum SignMode
    {
        PositiveAndNegative,
        OnlyPositive,
        OnlyNegative
    }

    [SerializeField] private SignMode _signMode = SignMode.PositiveAndNegative;
    [SerializeField, Min(0)] private int _maximumValue = int.MaxValue;

    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        if (_inputField == null)
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        // Character typed -> Validate it before accepting
        _inputField.onValidateInput += ValidateInput;
    }

    private void OnDisable()
    {
        if (_inputField != null)
        {
            _inputField.onValidateInput -= ValidateInput;
        }
    }

    private char ValidateInput(string currentText, int charIndex, char addedChar)
    {
        if (!IsSupportedCharacter(addedChar)) { return '\0'; }

        string candidateText = currentText.Insert(charIndex, addedChar.ToString());
        return IsValidCandidate(candidateText) ? addedChar : '\0';
    }

    private bool IsSupportedCharacter(char character)
    {
        if (char.IsDigit(character)) { return true; } // Digit -> Supported

        return character == '-' && _signMode != SignMode.OnlyPositive; // Minus sign + !OnlyPositive -> Supported
    }

    private bool IsValidCandidate(string candidateText)
    {
        if (string.IsNullOrEmpty(candidateText)) { return true; } // Empty String -> Valid

        bool hasNegativeSign = candidateText[0] == '-';

        if (hasNegativeSign)
        {
            if (_signMode == SignMode.OnlyPositive) { return false; } // Minus sign + OnlyPositive -> Invalid
            if (candidateText.Length == 1) { return true; } // Minus sign only -> Valid
            if (!HasValidDigits(candidateText, 1)) { return false; }

            string negativeDigits = candidateText.Substring(1);
            return IsWithinMaximumValue(negativeDigits);
        }

        if (_signMode == SignMode.OnlyNegative) { return false; }
        if (!HasValidDigits(candidateText, 0)) { return false; }

        return IsWithinMaximumValue(candidateText);
    }

    private static bool HasValidDigits(string value, int startIndex)
    {
        for (int index = startIndex; index < value.Length; index++)
        {
            if (!char.IsDigit(value[index]))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsWithinMaximumValue(string digits)
    {
        if (digits.Length == 0) { return true; } // Empty string -> Return True
        if (digits.Length > 1 && digits[0] == '0') { return false; } // Leading zeros -> Return False
        if (_signMode == SignMode.OnlyNegative && digits == "0") { return false;}
        if (_maximumValue == int.MaxValue) { return true; } // Maximum value is default -> Return True

        string maximumValueText = _maximumValue.ToString();

        if (digits.Length < maximumValueText.Length) { return true; }
        if (digits.Length > maximumValueText.Length) { return false; }

        return string.CompareOrdinal(digits, maximumValueText) <= 0;
    }
}
