﻿using FluentValidation.Validators;
using System;

namespace TauCode.Validation
{
    public class ExactDateValidator : PropertyValidator
    {
        private const string FORMAT = "yyyy-MM-dd";

        private readonly DateTime? _minDate;
        private readonly DateTime? _maxDate;

        public ExactDateValidator(DateTime? minDate, DateTime? maxDate, string message)
            : base(message)
        {
            if (minDate.HasValue && minDate.Value != minDate.Value.Date)
            {
                throw new ArgumentException($"When '{nameof(minDate)}' is provided, it must represent an exact date.", nameof(minDate));
            }

            if (maxDate.HasValue && maxDate.Value != maxDate.Value.Date)
            {
                throw new ArgumentException($"When '{nameof(maxDate)}' is provided, it must represent an exact date.", nameof(maxDate));
            }

            if (minDate.HasValue && maxDate.HasValue && minDate.Value > maxDate.Value)
            {
                throw new ArgumentException(
                    $"When both '{nameof(minDate)}' and '{nameof(maxDate)}' are provided, they must be sequential.",
                    nameof(maxDate));
            }

            _minDate = minDate;
            _maxDate = maxDate;

        }

        public ExactDateValidator(DateTime? minDate = null, DateTime? maxDate = null)
            : this(minDate, maxDate, "'{PropertyName}' must be an exact date{limitDescription}.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var date = (DateTime)context.PropertyValue;

            var valid = true;

            do
            {
                // must be exact
                if (date != date.Date)
                {
                    valid = false;
                    break;
                }

                // if min date provided, must adhere
                if (_minDate.HasValue)
                {
                    if (date < _minDate.Value)
                    {
                        valid = false;
                        break;
                    }
                }

                // if max date provided, must adhere
                if (_maxDate.HasValue)
                {
                    if (date > _maxDate.Value)
                    {
                        valid = false;
                        break;
                    }
                }

            } while (false);

            if (!valid)
            {
                string limitDescription;

                if (_minDate.HasValue && !_maxDate.HasValue)
                {
                    // _minDate..∞
                    limitDescription = $" not less than {_minDate.Value.ToString(FORMAT)}";
                }
                else if (!_minDate.HasValue && _maxDate.HasValue)
                {
                    // -∞.._maxDate
                    limitDescription = $" not greater than {_maxDate.Value.ToString(FORMAT)}";
                }
                else if (_minDate.HasValue && _maxDate.HasValue)
                {
                    // _minDate.._maxDate

                    if (_minDate.Value == _maxDate.Value)
                    {
                        limitDescription = $" equal to {_minDate.Value.ToString(FORMAT)}";
                    }
                    else
                    {
                        limitDescription = $" within range {_minDate.Value.ToString(FORMAT)}..{_maxDate.Value.ToString(FORMAT)}";
                    }
                }
                else
                {
                    // -∞..∞
                    limitDescription = "";
                }

                context.MessageFormatter
                    .AppendArgument("limitDescription", limitDescription);
            }

            return valid;
        }
    }
}
