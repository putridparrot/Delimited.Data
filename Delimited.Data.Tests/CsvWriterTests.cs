﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Delimited.Data.Exceptions;
using Delimited.Data.Specializations;
using Moq;
using Xunit;

namespace Delimited.Data.Tests
{
	[ExcludeFromCodeCoverage]
	public class CsvWriterTests
	{
		[Fact]
		public void CsvWriter_Ctor_NonWriteableStream()
		{
			var mock = new Mock<Stream>();
			mock.Setup(s => s.CanWrite).Returns(false);

			Assert.Throws<DelimitedStreamWriterException>(() => { CsvWriter writer = new CsvWriter(mock.Object); });
		}

		[Fact]
		public void CsvWriter_Ctor2_NonWriteableStream()
		{
			var mock = new Mock<Stream>();
			mock.Setup(s => s.CanWrite).Returns(false);

			Assert.Throws<DelimitedStreamWriterException>(() => { CsvWriter writer = new CsvWriter(mock.Object, Encoding.ASCII); });
		}

		[Fact]
		public void CsvWriter_CloseImplicitlyCalledViaDispose()
		{
			var mock = new Mock<Stream>();
			mock.Setup(s => s.CanWrite).Returns(true);
			mock.Setup(s => s.Close()).Verifiable();

			var writer = new CsvWriter((mock.Object));
			writer.Dispose();

			mock.Verify();
		}

		[Fact]
		public void CsvWriter_Close_ShouldCloseStream()
		{
			var mock = new Mock<Stream>();
			mock.Setup(s => s.CanWrite).Returns(true);
			mock.Setup(s => s.Close()).Verifiable();

			var writer = new CsvWriter(mock.Object);
			writer.Close();

			mock.Verify();
		}

		[Fact]
		public void CsvWriter_WriteLine()
		{
			var ms = new MemoryStream();

			var writer = new CsvWriter(ms);
			writer.WriteLine(new[] { "Hello", "World" });

			writer.Flush();
			ms.Position = 0;

			var reader = new StreamReader(ms);
			string result = reader.ReadToEnd();

			Assert.Equal("Hello,World\r\n", result);
		}

		[Fact]
		public void CsvReader_CheckOptionsSetterGetter()
		{
			var options = new DelimitedOptions('.');

			var writer = new CsvWriter(new MemoryStream()) {Options = options};

			Assert.Equal(options, writer.Options);
		}
	}

}
