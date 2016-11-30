using System;
using System.Text.RegularExpressions;
using Hive.Foundation.Extensions;

namespace Hive.Foundation.Entities
{
	/// <summary>
	/// A semantic version - conforms to SemVer 2.0.
	/// </summary>
	/// <remarks>http://semver.org</remarks>
	public class SemVer : IComparable<SemVer>, IComparable
	{
		public const string SemVerRegex = @"^(?<major>\d+)" +
		                                  @"(\.(?<minor>\d+))?" +
		                                  @"(\.(?<patch>\d+))?" +
		                                  @"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
		                                  @"(\+(?<build>[0-9A-Za-z\-\.]+))?$";

		private static readonly Regex ParseRegex = new Regex(SemVerRegex, RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		public static SemVer Parse(string version)
		{
			version.NotNullOrEmpty(nameof(version));

			var match = ParseRegex.Match(version);
			if (!match.Success)
				throw new ArgumentException("Invalid version.", nameof(version));

			var major = match.Groups["major"].Value.IntSafeInvariantParse();
			var minor = match.Groups["minor"].Value.IntSafeInvariantParse();
			var patch = match.Groups["patch"].Value.IntSafeInvariantParse();
			var prerelease = match.Groups["pre"].Value.IsNullOrEmpty() ? null : match.Groups["pre"].Value;
			var build = match.Groups["build"].Value.IsNullOrEmpty() ? null : match.Groups["build"].Value;

			return new SemVer(major ?? 0, minor ?? 0, patch ?? 0, prerelease, build);
		}

		public SemVer(int major, int minor, int patch, string prerelease = null, string build = null)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Prerelease = prerelease;
			Build = build;
		}

		public int Major { get; }

		public int Minor { get; }

		public int Patch { get; }

		public string Prerelease { get; }

		public string Build { get; }

		public int CompareTo(object obj)
		{
			return CompareTo(obj as SemVer);
		}

		public int CompareTo(SemVer other)
		{
			if (ReferenceEquals(other, null))
				return 1;

			var r = CompareByPrecedence(other);
			if (r != 0)
				return r;

			r = CompareComponent(Build, other.Build);
			return r;
		}

		public override string ToString() => $"{Major}.{Minor}.{Patch}{Prerelease.SafeInvariantFormat("-{0}")}{Build.SafeInvariantFormat("+{0}")}";

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			var other = obj as SemVer;

			return Major == other?.Major &&
				Minor == other.Minor &&
				Patch == other.Patch &&
				Prerelease.SafeOrdinalEquals(other.Prerelease) &&
				Build.SafeOrdinalEquals(other.Build);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = Major.GetHashCode();
				result = result * 31 + Minor.GetHashCode();
				result = result * 31 + Patch.GetHashCode();
				result = result * 31 + Prerelease.GetHashCode();
				result = result * 31 + Build.GetHashCode();
				return result;
			}
		}

		private int CompareByPrecedence(SemVer other)
		{
			if (ReferenceEquals(other, null))
				return 1;

			var r = Major.CompareTo(other.Major);
			if (r != 0) return r;

			r = Minor.CompareTo(other.Minor);
			if (r != 0) return r;

			r = Patch.CompareTo(other.Patch);
			if (r != 0) return r;

			r = CompareComponent(Prerelease, other.Prerelease, true);
			return r;
		}

		private static int CompareComponent(string a, string b, bool lower = false)
		{
			var aEmpty = string.IsNullOrEmpty(a);
			var bEmpty = string.IsNullOrEmpty(b);
			if (aEmpty && bEmpty)
				return 0;

			if (aEmpty)
				return lower ? 1 : -1;
			if (bEmpty)
				return lower ? -1 : 1;

			var aComps = a.Split('.');
			var bComps = b.Split('.');

			var minLen = Math.Min(aComps.Length, bComps.Length);
			for (var i = 0; i < minLen; i++)
			{
				var ac = aComps[i];
				var bc = bComps[i];
				int anum, bnum;
				var isanum = int.TryParse(ac, out anum);
				var isbnum = int.TryParse(bc, out bnum);
				int r;
				if (isanum && isbnum)
				{
					r = anum.CompareTo(bnum);
					if (r != 0) return anum.CompareTo(bnum);
				}
				else
				{
					if (isanum)
						return -1;
					if (isbnum)
						return 1;
					r = string.CompareOrdinal(ac, bc);
					if (r != 0)
						return r;
				}
			}

			return aComps.Length.CompareTo(bComps.Length);
		}
	}
}
