using System;

namespace IlemlamlaBlazor.Services
{
    public class LanguageState
    {
        private string _language = "pl";
        public string Language
        {
            get => _language;
            set
            {
                if (_language != value)
                {
                    _language = value;
                    NotifyLanguageChanged();
                }
            }
        }

        public event Action? OnChange;

        private void NotifyLanguageChanged() => OnChange?.Invoke();

        public void Register(Action callback)
        {
            OnChange += callback;
        }

        public void Unregister(Action callback)
        {
            OnChange -= callback;
        }
    }
}