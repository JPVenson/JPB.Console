using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#if !NET_CORE

#endif

namespace JPB.Console.Helper.Grid
{
#if !NET_CORE
	[Serializable]
#endif
	public struct ColoredString : IEquatable<ColoredString>, IXmlSerializable
#if !NET_CORE
		, ISerializable
#endif

	{
		private string _text;
		private ConsoleColor? _forgroundColor;
		private ConsoleColor? _backgroundColor;

		public ColoredString(string text, ConsoleColor? forgroundColor = null, ConsoleColor? backgroundColor = null)
		{
			_forgroundColor = forgroundColor;
			_backgroundColor = backgroundColor;
			_text = text;
		}

#if !NET_CORE
		public ColoredString(SerializationInfo info, StreamingContext context)
		{
			_text = info.GetString("text");
			_forgroundColor = (ConsoleColor?) info.GetInt32("fcolor");
			_backgroundColor = (ConsoleColor?) info.GetInt32("bcolor");
		}
#endif

		public ConsoleColor? GetRefColor()
		{
			return _backgroundColor & _backgroundColor;
		}

		public ConsoleColor? GetForgroundColor()
		{
			return _forgroundColor;
		}

		public ConsoleColor? GetBackgroundColor()
		{
			return _backgroundColor;
		}

		public override string ToString()
		{
			return _text;
		}

		public bool ColorEquals(ColoredString other)
		{
			return _forgroundColor == other._forgroundColor && _backgroundColor == other._backgroundColor;
		}

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <returns>
		///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise,
		///     false.
		/// </returns>
		/// <param name="obj">The object to compare with the current instance. </param>
		public override bool Equals(object obj)
		{
			if (obj is ColoredString)
			{
				var compareTo = (ColoredString) obj;
				return ColorEquals(compareTo) && _text == compareTo._text;
			}

			return base.Equals(obj);
		}

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ColoredString other)
		{
			return string.Equals(_text, other._text) && ColorEquals(other);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _text != null ? _text.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ _forgroundColor.GetHashCode();
				hashCode = (hashCode * 397) ^ _backgroundColor.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(ColoredString left, ColoredString right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ColoredString left, ColoredString right)
		{
			return !left.Equals(right);
		}


#if !NET_CORE
		/// <summary>
		///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize
		///     the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
		/// <param name="context">
		///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
		///     serialization.
		/// </param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("fcolor", _forgroundColor);
			info.AddValue("bcolor", _backgroundColor);
			info.AddValue("text", _text);
		}
#endif

		/// <summary>
		///     This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
		///     return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required,
		///     apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
		///     produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
		///     and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
		///     method.
		/// </returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>Generates an object from its XML representation.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("ColordString");
			_backgroundColor = GetColorFromAttribute(reader, "background");
			_forgroundColor = GetColorFromAttribute(reader, "forground");
			_text = reader.GetAttribute("text");
			reader.ReadEndElement();
		}

		private ConsoleColor? GetColorFromAttribute(XmlReader reader, string attributeName)
		{
			var attribute = reader.GetAttribute(attributeName);
			if (string.IsNullOrWhiteSpace(attribute))
			{
				return null;
			}

			ConsoleColor backgValue;
			if (!Enum.TryParse(attribute, out backgValue))
			{
				return null;
			}

			return backgValue;
		}

		/// <summary>Converts an object into its XML representation.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ColordString");
			writer.WriteAttributeString("background",
				_backgroundColor.HasValue ? _backgroundColor.Value.ToString() : "");
			writer.WriteAttributeString("forground", _forgroundColor.HasValue ? _forgroundColor.Value.ToString() : "");
			writer.WriteAttributeString("text", _text);
			writer.WriteEndElement();
		}
	}

#if !NET_CORE
	[Serializable]
#endif
	public class StringBuilderInterlaced : IXmlSerializable, IEnumerable<ColoredString>
#if !NET_CORE
		, ISerializable
#endif

	{
		private readonly uint _interlacedSpace = 4;
		private readonly List<ColoredString> _source;
		private readonly bool _transformInterlaced;
		private ConsoleColor? _backgroundColor;
		private ConsoleColor? _forgroundColor;
		private int _interlacedLevel;

		/// <summary>
		/// </summary>
		/// <param name="transformInterlaced">If true an level will be displaced as <paramref name="intedtSize" /> spaces</param>
		/// <param name="intedtSize">ammount of spaces for each level</param>
		public StringBuilderInterlaced(bool transformInterlaced = false, uint intedtSize = 4)
		{
			_interlacedSpace = intedtSize;
			_transformInterlaced = transformInterlaced;
			_source = new List<ColoredString>();
		}

		public int Length => _source.Count;

		public IEnumerator<ColoredString> GetEnumerator()
		{
			return _source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _source).GetEnumerator();
		}

#if !NET_CORE
		/// <summary>
		///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize
		///     the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
		/// <param name="context">
		///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
		///     serialization.
		/// </param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Compact();
			info.AddValue("count", _source.Count);
			for (var index = 0; index < _source.Count; index++)
			{
				var coloredString = _source[index];
				info.AddValue("item" + index, coloredString);
			}
		}
#endif

		/// <summary>
		///     This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should
		///     return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required,
		///     apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
		///     produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
		///     and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
		///     method.
		/// </returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>Generates an object from its XML representation.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("Sbi");
			reader.ReadStartElement("ColoredString");
			while (reader.Value == "ColoredString")
			{
				var elment = new ColoredString();
				elment.ReadXml(reader);
				_source.Add(elment);
			}

			reader.ReadEndElement();
		}

		/// <summary>Converts an object into its XML representation.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Sbi");
			foreach (var coloredString in _source)
			{
				coloredString.WriteXml(writer);
			}

			writer.WriteEndElement();
		}

		public StringBuilderInterlaced ForgroundColor(ConsoleColor color)
		{
			_forgroundColor = color;
			return this;
		}

		public StringBuilderInterlaced BackgroundColor(ConsoleColor color)
		{
			_backgroundColor = color;
			return this;
		}

		public StringBuilderInterlaced RevertColor()
		{
			_forgroundColor = null;
			_backgroundColor = null;
			return this;
		}

		public StringBuilderInterlaced Up()
		{
			_interlacedLevel++;
			return this;
		}

		public StringBuilderInterlaced Down()
		{
			if (_interlacedLevel > 0)
			{
				_interlacedLevel--;
			}

			return this;
		}

		public StringBuilderInterlaced AppendLine()
		{
			return Append(Environment.NewLine);
		}

		private void ApplyLevel()
		{
			var text = "";
			if (_transformInterlaced)
			{
				for (var i = 0; i < _interlacedLevel; i++)
				{
					for (var j = 0; j < _interlacedSpace; j++)
					{
						text += " ";
					}
				}
			}
			else
			{
				for (var i = 0; i < _interlacedLevel; i++)
				{
					text += "\t";
				}
			}

			_source.Add(new ColoredString(text));
		}

		public StringBuilderInterlaced AppendInterlacedLine(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			ApplyLevel();
			return AppendLine(value, forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced AppendInterlaced(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			ApplyLevel();
			return Append(value, forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced AppendInterlacedLine(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null, params object[] values)
		{
			return AppendInterlacedLine(string.Format(value, values), forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced AppendInterlaced(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null, params object[] values)
		{
			return AppendInterlaced(string.Format(value, values), forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced AppendInterlacedLine(string value, params object[] values)
		{
			return AppendInterlacedLine(string.Format(value, values));
		}

		public StringBuilderInterlaced AppendInterlaced(string value, params object[] values)
		{
			return AppendInterlaced(string.Format(value, values));
		}

		public StringBuilderInterlaced Insert(Action<StringBuilderInterlaced> del)
		{
			del(this);
			return this;
		}

		public StringBuilderInterlaced Insert(StringBuilderInterlaced writer)
		{
			foreach (var coloredString in writer._source)
			{
				_source.Add(coloredString);
			}

			return this;
		}

		public StringBuilderInterlaced Append(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			if (!forgroundColor.HasValue)
			{
				forgroundColor = _forgroundColor;
			}

			if (!backgroundColor.HasValue)
			{
				backgroundColor = _backgroundColor;
			}

			_source.Add(new ColoredString(value, forgroundColor, backgroundColor));
			return this;
		}

		public StringBuilderInterlaced Append(char value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			return Append(value.ToString(), forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced AppendLine(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null)
		{
			return Append(value + Environment.NewLine, forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced Append(string value, params object[] values)
		{
			return Append(string.Format(value, values));
		}

		public StringBuilderInterlaced AppendLine(string value, params object[] values)
		{
			return Append(string.Format(value, values) + Environment.NewLine);
		}

		public StringBuilderInterlaced Append(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null, params object[] values)
		{
			return Append(string.Format(value, values), forgroundColor, backgroundColor);
		}

		public StringBuilderInterlaced AppendLine(string value, ConsoleColor? forgroundColor = null,
			ConsoleColor? backgroundColor = null, params object[] values)
		{
			return Append(string.Format(value, values) + Environment.NewLine, forgroundColor, backgroundColor);
		}

		public void WriteToSteam(TextWriter output, Action<ConsoleColor?, ConsoleColor?> colorChanged,
			Action changeColorBack)
		{
			Compact();
			ColoredString? lastElement = null;
			var sb = new StringBuilder();
			foreach (var coloredString in _source)
			{
				if (!lastElement.HasValue)
				{
					lastElement = coloredString;
					sb.Append(coloredString.ToString());
					continue;
				}

				if (lastElement.Value.ColorEquals(coloredString))
				{
					sb.Append(coloredString.ToString());
				}
				else
				{
					if (sb.Length == 0)
					{
						sb.Append(lastElement.Value.ToString());
					}

					colorChanged(lastElement.Value.GetForgroundColor(), lastElement.Value.GetBackgroundColor());
					output.Write(sb.ToString());
					changeColorBack();
					sb.Clear();
				}

				lastElement = coloredString;
			}

			if (lastElement.HasValue)
			{
				if (lastElement != null)
				{
					colorChanged(lastElement.Value.GetForgroundColor(), lastElement.Value.GetBackgroundColor());
				}

				output.Write(lastElement.Value.ToString());
				changeColorBack();
				sb.Clear();
			}
		}

		public void WriteToConsole(bool clearConsole = false)
		{
			var forg = System.Console.ForegroundColor;
			var backg = System.Console.BackgroundColor;

			System.Console.CursorVisible = false;
			using (var bufferedConsole = new DoubleBufferConsole.DoubleBufferConsole())
			{
				WriteToSteam(bufferedConsole, (forground, background) =>
				{
					if (forground.HasValue)
					{
						bufferedConsole.ForegroundColor = forground.Value;
					}

					if (background.HasValue)
					{
						bufferedConsole.BackgroundColor = background.Value;
					}
				}, () =>
				{
					bufferedConsole.ForegroundColor = forg;
					bufferedConsole.BackgroundColor = backg;
				});
				bufferedConsole.Flush(clearConsole);
			}

			System.Console.CursorVisible = true;


			//System.Console.CursorVisible = false;
			//WriteToSteam(System.Console.Out, (forground, background) =>
			//{
			//	if (forground.HasValue)
			//		System.Console.ForegroundColor = forground.Value;
			//	if (background.HasValue)
			//		System.Console.BackgroundColor = background.Value;
			//}, () =>
			//{
			//	System.Console.ForegroundColor = forg;
			//	System.Console.BackgroundColor = backg;
			//});

			//System.Console.CursorVisible = true;
		}

		public override string ToString()
		{
			return _source.Select(f => f.ToString()).ToString();
		}

		/// <summary>
		/// Compacts this instance. This will aggregate all colores that are near to each other into one.
		/// </summary>
		/// <returns>Number of compacted strings</returns>
		public int Compact()
		{
			var nSource = new List<ColoredString>();
			var merged = 0;
			ColoredString? lastItem = null;
			ColoredString? newItem = null;

			if (Length == 1)
			{
				return 0;
			}

			foreach (var coloredString in _source)
			{
				if (!lastItem.HasValue)
				{
					lastItem = coloredString;
					continue;
				}

				if (coloredString.GetRefColor() == lastItem.Value.GetRefColor())
				{
					merged++;
					if (!newItem.HasValue)
					{
						newItem = new ColoredString(lastItem.Value + coloredString.ToString(),
							coloredString.GetForgroundColor(), coloredString.GetBackgroundColor());
					}
					else
					{
						newItem = new ColoredString(newItem.Value + coloredString.ToString(),
							coloredString.GetForgroundColor(), coloredString.GetBackgroundColor());
					}
				}
				else
				{
					if (newItem.HasValue)
					{
						nSource.Add(newItem.Value);
						newItem = null;
					}
					else
					{
						nSource.Add(lastItem.Value);
					}
				}

				lastItem = coloredString;
			}

			if (newItem.HasValue)
			{
				nSource.Add(newItem.Value);
			}

			_source.Clear();
			_source.AddRange(nSource);

			return merged;
		}
#if !NET_CORE

		private StringBuilderInterlaced()
		{
		}

		public StringBuilderInterlaced(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("count");

			for (var i = 0; i < count; i++)
			{
				var element = (ColoredString) info.GetValue("item" + i, typeof(ColoredString));
				_source.Add(element);
			}
		}
#endif
	}

	public static class StringBuilderInterlacedExtentions
	{
		public static void WriteToStreamAsHtml(this TextWriter writer, StringBuilderInterlaced source)
		{
			source.WriteToSteam(writer, (forg, backg) =>
			{
				var color = "";
				if (forg.HasValue)
				{
					color += "color=" + forg.Value;
				}

				if (backg.HasValue)
				{
					if (forg.HasValue)
					{
						color += ",";
					}

					color += "background-color=" + backg.Value;
				}

				writer.Write("<p style=\"{0}\">", color);
			}, () => { writer.Write("</p>"); });
		}
	}
}