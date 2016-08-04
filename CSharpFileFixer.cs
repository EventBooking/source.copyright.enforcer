using System;
using System.IO;

namespace SourceCopyrightEnforcer
{
	internal class CSharpFileFixer
	{
		private const string CopyrightPrefix = "// Copyright ";
		private readonly FileInfo _file;
		private readonly string _lines;
		private readonly int _thisYear;
		private readonly int? _startYear;
		private readonly string _firstLine;

		public CSharpFileFixer(FileInfo file)
		{
			_file = file;
			_lines = File.ReadAllText(_file.FullName);
			_thisYear = DateTime.Now.Year;
			_firstLine = ExistingFirstLine;
			_startYear = GetStartYear();
		}

		public bool NeedsFix
		{
			get
			{
				try
				{
					if (!IsFirstLineCorrect) return false;

					return true;
				}
				catch (Exception)
				{
					// Don't bother with empty file
					return false;
				}
			}
		}

		private bool IsFirstLineCorrect
		{
			get
			{
				var expectedFirstLine = DesiredFirstLine;
				if (_firstLine == expectedFirstLine)
					return false;
				return true;
			}
		}

		private int? GetStartYear()
		{
			if (IsFirstLineCopyright)
			{
				var parts = _firstLine.Split(' ');
				var years = parts[2];
				if (string.IsNullOrEmpty(years))
					return null;
				var yearParts = years.Split('-');
				if (yearParts.Length < 1)
					return null;

				int startYear;
				var success = Int32.TryParse(yearParts[0], out startYear);
				if (success)
				{
					return startYear;
				}
			}
			return null;
		}

		private bool IsFirstLineCopyright
		{
			get { return _firstLine.Contains(CopyrightPrefix); }
		}

		private string DesiredFirstLine
		{
			get
			{
				if (!_startYear.HasValue || _thisYear == _startYear)
					return string.Format("// Copyright {0} EventBooking.com, LLC. All rights reserved.", _thisYear);
				return string.Format("// Copyright {0}-{1} EventBooking.com, LLC. All rights reserved.", _startYear, _thisYear);
			}
		}

		private bool NeedsNewFirstLine
		{
			get
			{
				if (NeedsFix)
				{
					return !IsFirstLineCopyright;
				}
				return false;
			}
		}

		public void Fix()
		{
			// First line should be something like
			// Copyright 2007-2009 EventBooking.com, LLC. All rights reserved.
			string newContent;

			if (NeedsNewFirstLine)
				newContent = DesiredFirstLine + Environment.NewLine + Environment.NewLine + _lines;
			else
				newContent = _lines.Replace(ExistingFirstLine, DesiredFirstLine);

			File.WriteAllText(_file.FullName, newContent);
		}

		private string ExistingFirstLine
		{
			get
			{
				string[] lines = _lines.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
				if (lines.Length < 2)
					throw new Exception("File is empty");
				return lines[0];
			}
		}
	}
}