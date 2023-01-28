﻿namespace Stas.Utils
{
	/// <summary>
	/// Ephemeral values used during the SRP-6a authentication.
	/// </summary>
	public class SrpEphemeral
	{
		/// <summary>
		/// Gets or sets the public part.
		/// </summary>
		public string Public { get; set; }

		/// <summary>
		/// Gets or sets the secret part.
		/// </summary>
		public string Secret { get; set; }
	}
}
