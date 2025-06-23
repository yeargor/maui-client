using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiDemo2.Models
{
    public class RouteMainPage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    DescriptionParts = SplitText(value, 200);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DescriptionParts));
                }
            }
        }

        public List<string> DescriptionParts { get; private set; } = new List<string>();

        private int _timesCompleted;
        public int TimesCompleted
        {
            get => _timesCompleted;
            set
            {
                if (_timesCompleted != value)
                {
                    _timesCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        private float _rating;
        public float Rating
        {
            get => _rating;
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _distance;
        public double Distance
        {
            get => _distance;
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _daysAgo;
        public string DaysAgo
        {
            get => _daysAgo;
            set
            {
                if (_daysAgo != value)
                {
                    _daysAgo = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSaved;
        public bool IsSaved
        {
            get => _isSaved;
            set
            {
                if (_isSaved != value)
                {
                    _isSaved = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFinished;
        public bool IsFinished
        {
            get => _isFinished;
            set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    OnPropertyChanged();
                }
            }
        }

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> _images = new List<string> { "white.jpg", "white.jpg", "white.jpg" };
        public List<string> Images
        {
            get => _images;
            set
            {
                if (_images != value)
                {
                    _images = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> SplitText(string text, int chunkSize)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            var parts = new List<string>();
            for (int i = 0; i < text.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, text.Length - i);
                parts.Add(text.Substring(i, length));
            }
            return parts;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            System.Diagnostics.Debug.WriteLine($"PropertyChanged triggered for {propertyName}");
        }
    }
}