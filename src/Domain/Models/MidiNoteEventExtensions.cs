using System;

namespace NAudioVisualizer.Domain.Models
{
	/// <summary>
	/// Provides extension methods for <see cref="MidiNoteEvent"/> to simplify common MIDI event operations.
	/// </summary>
	public static class MidiNoteEventExtensions
	{
		/// <summary>
		/// Determines if a MidiNoteEvent represents a note being pressed (velocity > 0).
		/// </summary>
		/// <param name="event">The MidiNoteEvent to check.</param>
		/// <returns>True if the event represents a note being pressed; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/>.</exception>
		public static bool IsNotePressed(this MidiNoteEvent @event)
		{
			ArgumentNullException.ThrowIfNull(@event);
			return @event.IsNoteOn && @event.Velocity > 0;
		}

		/// <summary>
		/// Determines if a MidiNoteEvent represents a note being released (velocity == 0 or note on == false).
		/// </summary>
		/// <param name="event">The MidiNoteEvent to check.</param>
		/// <returns>True if the event represents a note being released; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/>.</exception>
		public static bool IsNoteReleased(this MidiNoteEvent @event)
		{
			ArgumentNullException.ThrowIfNull(@event);
			return !@event.IsNoteOn || @event.Velocity == 0;
		}

		/// <summary>
		/// Gets a human-readable string representation of the MidiNoteEvent.
		/// </summary>
		/// <param name="event">The MidiNoteEvent to convert to a string.</param>
		/// <returns>A string in the format: "{NoteName} (Channel {Channel}, Velocity {Velocity})"</returns>
		/// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/>.</exception>
		public static string ToReadableString(this MidiNoteEvent @event)
		{
			ArgumentNullException.ThrowIfNull(@event);
			return $"{@event.NoteName} (Channel {@event.Channel}, Velocity {@event.Velocity})" ;
		}
	}
}
