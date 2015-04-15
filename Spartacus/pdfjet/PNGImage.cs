/**
 *  PNGImage.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and / or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;


/**
 * Used to embed PNG images in the PDF document.
 *
 */
namespace PDFjet.NET {
public class PNGImage {

    int w = 0;              // Image width in pixels
    int h = 0;              // Image height in pixels

    byte[] data;            // The compressed data in the IDAT chunk
    byte[] inflated;        // The decompressed image data
    byte[] image;           // The reconstructed image data
    byte[] deflated;        // The deflated reconstructed image data
    byte[] rgb;             // The palette data
    byte[] alphaForPalette; // The alpha for the palette data
    byte[] deflatedAlpha;   // The deflated alpha channel data

    private byte bitDepth = 8;
    private int colorType = 0;


    /**
     * Used to embed PNG images in the PDF document.
     *
     */
    public PNGImage(Stream inputStream) {
        ValidatePNG(inputStream);

        List<Chunk> chunks = new List<Chunk>();
        ProcessPNG(chunks, inputStream);

        for (int i = 0; i < chunks.Count; i++) {
            Chunk chunk = chunks[i];
            if (chunk.type[0] == 'I'
                    && chunk.type[1] == 'H'
                    && chunk.type[2] == 'D'
                    && chunk.type[3] == 'R') {

                this.w = ToIntValue(chunk.GetData(), 0);    // Width
                this.h = ToIntValue(chunk.GetData(), 4);    // Height
                this.bitDepth = chunk.GetData()[8];         // Bit Depth
                this.colorType = chunk.GetData()[9];        // Color Type

                // Console.WriteLine(
                //         "Bit Depth == " + chunk.GetData()[ 8 ] );
                // Console.WriteLine(
                //         "Color Type == " + chunk.GetData()[ 9 ] );
                // Console.WriteLine( chunk.GetData()[ 10 ] );
                // Console.WriteLine( chunk.GetData()[ 11 ] );
                // Console.WriteLine( chunk.GetData()[ 12 ] );

            }
            else if (chunk.type[0] == 'I'
                    && chunk.type[1] == 'D'
                    && chunk.type[2] == 'A'
                    && chunk.type[3] == 'T' ) {
                data = AppendIdatChunk(data, chunk.GetData());
            }
            else if (chunk.type[0] == 'P'
                    && chunk.type[1] == 'L'
                    && chunk.type[2] == 'T'
                    && chunk.type[3] == 'E') {
                rgb = chunk.GetData();
                if (rgb.Length % 3 != 0) {
                    throw new Exception("Incorrect palette length.");
                }
            }
            else if (chunk.type[0] == 'g'
                    && chunk.type[1] == 'A'
                    && chunk.type[2] == 'M'
                    && chunk.type[3] == 'A') {
                // TODO:
                // Console.WriteLine("gAMA chunk found!");
            }
            else if (chunk.type[0] == 't'
                    && chunk.type[1] == 'R'
                    && chunk.type[2] == 'N'
                    && chunk.type[3] == 'S') {
                // Console.WriteLine("tRNS chunk found!");
                if (colorType == 3) {
                    alphaForPalette = new byte[this.w * this.h];
                    for (int j = 0; j < alphaForPalette.Length; j++) { alphaForPalette[j] = (byte) 0xff; }
                    byte[] alpha = chunk.GetData();
                    for (int j = 0; j < alpha.Length; j++) {
                        alphaForPalette[j] = alpha[j];
                    }
                }
            }
            else if (chunk.type[0] == 'c'
                    && chunk.type[1] == 'H'
                    && chunk.type[2] == 'R'
                    && chunk.type[3] == 'M') {
                // TODO:
                // Console.WriteLine("cHRM chunk found!");
            }
            else if (chunk.type[0] == 's'
                    && chunk.type[1] == 'B'
                    && chunk.type[2] == 'I'
                    && chunk.type[3] == 'T') {
                // TODO:
                // Console.WriteLine("sBIT chunk found!");
            }
            else if (chunk.type[0] == 'b'
                    && chunk.type[1] == 'K'
                    && chunk.type[2] == 'G'
                    && chunk.type[3] == 'D') {
                // TODO:
                // Console.WriteLine("bKGD chunk found!");
            }

        }

        inflated = GetDecompressedData();

        if (colorType == 0) {
            // Grayscale Image
            if (bitDepth == 16) {
                image = getImageColorType0BitDepth16();
            }
            else if (bitDepth == 8 ) {
                image = getImageColorType0BitDepth8();
            }
            else if (bitDepth == 4) {
                image = getImageColorType0BitDepth4();
            }
            else if (bitDepth == 2) {
                image = getImageColorType0BitDepth2();
            }
            else if (bitDepth == 1) {
                image = getImageColorType0BitDepth1();
            }
            else {
                throw new Exception("Image with unsupported bit depth == " + bitDepth);
            }
        }
        else if (colorType == 6) {
            image = getImageColorType6BitDepth8();
        }
        else {
            // Color Image
            if (rgb == null) {
                // Trucolor Image
                if (bitDepth == 16) {
                    image = getImageColorType2BitDepth16();
                }
                else {
                    image = getImageColorType2BitDepth8();
                }
            }
            else {
                // Indexed Image
                if (bitDepth == 8) {
                    image = getImageColorType3BitDepth8();
                }
                else if (bitDepth == 4) {
                    image = getImageColorType3BitDepth4();
                }
                else if (bitDepth == 2) {
                    image = getImageColorType3BitDepth2();
                }
                else if (bitDepth == 1) {
                    image = getImageColorType3BitDepth1();
                }
                else {
                    throw new Exception("Image with unsupported bit depth == " + bitDepth);
                }
            }
        }

        deflated = DeflateReconstructedData();
    }


    public int GetWidth() {
        return this.w;
    }


    public int GetHeight() {
        return this.h;
    }


    public int GetColorType() {
        return this.colorType;
    }


    public int GetBitDepth() {
        return this.bitDepth;
    }


    public byte[] GetData() {
        return this.deflated;
    }


    public byte[] GetAlpha() {
        return this.deflatedAlpha;
    }


    private void ProcessPNG(
            List< Chunk> chunks, System.IO.Stream inputStream) {

        while (true) {
            Chunk chunk = getChunk(inputStream);
            chunks.Add( chunk );
            if (chunk.type[0] == 'I'
                    && chunk.type[1] == 'E'
                    && chunk.type[2] == 'N'
                    && chunk.type[3] == 'D') {
                break;
            }
        }

    }


    private void ValidatePNG(Stream inputStream) {
        byte[] buf = new byte[8];
        if (inputStream.Read(buf, 0, buf.Length) == -1) {
            throw new Exception("File is too short!");
        }

        if ((buf[0] & 0xFF) == 0x89 &&
                buf[1] == 0x50 &&
                buf[2] == 0x4E &&
                buf[3] == 0x47 &&
                buf[4] == 0x0D &&
                buf[5] == 0x0A &&
                buf[6] == 0x1A &&
                buf[7] == 0x0A) {
            // The PNG signature is correct.
        }
        else {
            throw new Exception("Wrong PNG signature.");
        }
    }


    private Chunk getChunk(System.IO.Stream inputStream) {
        Chunk chunk = new Chunk();
        chunk.SetLength(GetLong(inputStream));      // The length of the data chunk.
        chunk.SetType(GetBytes(inputStream, 4));    // The chunk type.
        chunk.SetData(GetBytes(inputStream, chunk.GetLength()));    // The chunk data.
        chunk.SetCrc(GetLong(inputStream));         // CRC of the type and data chunks.
        if (!chunk.hasGoodCRC()) {
            throw new Exception("Chunk has bad CRC.");
        }
        return chunk;
    }


    private long GetLong(System.IO.Stream inputStream) {
        byte[] buf = GetBytes(inputStream, 4);
        return (ToIntValue(buf, 0) & 0x00000000ffffffffL);
    }


    private byte[] GetBytes(System.IO.Stream inputStream, long length) {
        byte[] buf = new byte[(int) length];
        if (inputStream.Read(buf, 0, buf.Length) == -1) {
            throw new Exception("Error reading input stream!");
        }

        return buf;
    }


    private int ToIntValue(byte[] buf, int off) {
        long val = 0L;

        val |= (long) buf[off] & 0xff;
        val <<= 8;
        val |= (long) buf[1 + off] & 0xff;
        val <<= 8;
        val |= (long) buf[2 + off] & 0xff;
        val <<= 8;
        val |= (long) buf[3 + off] & 0xff;

        return (int) val;
    }


    // Truecolor Image with Bit Depth == 16
    private byte[] getImageColorType2BitDepth16() {
        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        byte filter = 0x00;
        int scanLineLength = 6 * this.w;

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % ( scanLineLength + 1 ) == 0 ) {
                filter = inflated[ i ];
                continue;
            }

            image[ j ] = inflated[ i ];

            int a = 0;
            int b = 0;
            int c = 0;

            if ( j % scanLineLength >= 6 ) {
                a = ( image[ j - 6 ] & 0x000000ff );
            }

            if ( j >= scanLineLength ) {
                b = ( image[ j - scanLineLength ] & 0x000000ff );
            }

            if ( j % scanLineLength >= 6 && j >= scanLineLength) {
                c = ( image[ j - ( scanLineLength + 6 ) ] & 0x000000ff );
            }

            applyFilters( filter, image, j, a, b, c );

            j++;
        }

        return image;
    }


    // Truecolor Image with Bit Depth == 8
    private byte[] getImageColorType2BitDepth8() {

        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        byte filter = 0x00;
        int scanLineLength = 3 * this.w;

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % ( scanLineLength + 1 ) == 0 ) {
                filter = inflated[ i ];
                continue;
            }

            image[ j ] = inflated[ i ];

            int a = 0;
            int b = 0;
            int c = 0;

            if ( j % scanLineLength >= 3 ) {
                a = ( image[ j - 3 ] & 0x000000ff );
            }

            if ( j >= scanLineLength ) {
                b = ( image[ j - scanLineLength ] & 0x000000ff );
            }

            if ( j % scanLineLength >= 3 && j >= scanLineLength ) {
                c = ( image[ j - ( scanLineLength + 3 ) ] & 0x000000ff );
            }

            applyFilters( filter, image, j, a, b, c );

            j++;
        }

        return image;
    }


    // Truecolor Image with Alpha Transparency
    private byte[] getImageColorType6BitDepth8() {
        int j = 0;
        byte[] image = new byte[4 * this.w * this.h];

        byte filter = 0x00;
        int scanLineLength = 4 * this.w;

        for (int i = 0; i < inflated.Length; i++) {
            if (i % ( scanLineLength + 1) == 0) {
                filter = inflated[i];
                continue;
            }

            image[j] = inflated[i];

            int a = 0;
            int b = 0;
            int c = 0;

            if (j % scanLineLength >= 4 ) {
                a = (image[j - 4] & 0x000000ff);
            }
            if (j >= scanLineLength ) {
                b = (image[j - scanLineLength] & 0x000000ff );
            }
            if (j % scanLineLength >= 4 && j >= scanLineLength) {
                c = (image[j - (scanLineLength + 4 )] & 0x000000ff);
            }

            applyFilters(filter, image, j, a, b, c);
            j++;
        }

        byte[] idata = new byte[ 3 * this.w * this.h ]; // Image data
        byte[] alpha = new byte[this.w * this.h];       // Alpha values

        int k = 0;
        int n = 0;
        for (int i = 0; i < image.Length; i += 4) {
            idata[k]     = image[i];
            idata[k + 1] = image[i + 1];
            idata[k + 2] = image[i + 2];
            alpha[n]     = image[i + 3];

            k += 3;
            n += 1;
        }

        Compressor compressor = new Compressor(alpha);
        deflatedAlpha = compressor.GetCompressedData();

        return idata;
    }


    // Indexed Image with Bit Depth == 8
    private byte[] getImageColorType3BitDepth8() {
        int j = 0;
        int n = 0;

        byte[] alpha = null;
        if (alphaForPalette != null) {
            alpha = new byte[this.w * this.h];
        }

        byte[] image = new byte[3*(inflated.Length - this.h)];
        int scanLineLength = this.w + 1;
        for (int i = 0; i < inflated.Length; i++) {
            if (i % scanLineLength == 0) {
                // Skip the filter byte
                continue;
            }

            int k = ((int) inflated[i] & 0x000000ff);
            image[j++] = rgb[3*k];
            image[j++] = rgb[3*k + 1];
            image[j++] = rgb[3*k + 2];

            if (alphaForPalette != null) {
                alpha[n++] = alphaForPalette[k];
            }
        }

        if (alphaForPalette != null) {
            Compressor compressor = new Compressor(alpha);
            deflatedAlpha = compressor.GetCompressedData();
        }

        return image;
    }


    // Indexed Image with Bit Depth == 4
    private byte[] getImageColorType3BitDepth4() {

        int j = 0;
        int k = 0;

        byte[] image = new byte[ 6 * ( inflated.Length - this.h ) ];
        int scanLineLength = this.w / 2 + 1;
        if ( this.w % 2 > 0 ) {
            scanLineLength += 1;
        }

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % scanLineLength == 0 ) {
                // Skip the filter byte.
                continue;
            }

            int l = ( int ) inflated[ i ];

            k = 3 * ( ( l >> 4 ) & 0x0000000f );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 0 ) & 0x0000000f );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

        }

        return image;
    }


    // Indexed Image with Bit Depth == 2
    private byte[] getImageColorType3BitDepth2() {

        int j = 0;
        int k = 0;

        byte[] image = new byte[ 12 * ( inflated.Length - this.h ) ];
        int scanLineLength = this.w / 4 + 1;
        if ( this.w % 4 > 0 ) {
            scanLineLength += 1;
        }

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % scanLineLength == 0 ) {
                // Skip the filter byte.
                continue;
            }

            int l = ( int ) inflated[ i ];

            k = 3 * ( ( l >> 6 ) & 0x00000003 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 4 ) & 0x00000003 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 2 ) & 0x00000003 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 0 ) & 0x00000003 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

        }

        return image;
    }


    // Indexed Image with Bit Depth == 1
    private byte[] getImageColorType3BitDepth1() {

        int j = 0;
        int k = 0;

        byte[] image = new byte[ 24 * ( inflated.Length - this.h ) ];
        int scanLineLength = this.w / 8 + 1;
        if ( this.w % 8 > 0 ) {
            scanLineLength += 1;
        }

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % scanLineLength == 0 ) {
                // Skip the filter byte.
                continue;
            }

            int l = ( int ) inflated[ i ];

            k = 3 * ( ( l >> 7 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 6 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 5 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 4 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 3 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 2 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 1 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

            if ( j % ( 3 * this.w ) == 0 ) continue;

            k = 3 * ( ( l >> 0 ) & 0x00000001 );
            image[ j++ ] = rgb[ k ];
            image[ j++ ] = rgb[ k + 1 ];
            image[ j++ ] = rgb[ k + 2 ];

        }

        return image;
    }


    // Grayscale Image with Bit Depth == 16
    private byte[] getImageColorType0BitDepth16() {

        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        byte filter = 0x00;
        int scanLineLength = 2 * this.w;

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % ( scanLineLength + 1 ) == 0 ) {
                filter = inflated[ i ];
                continue;
            }

            image[ j ] = inflated[ i ];

            int a = 0;
            int b = 0;
            int c = 0;

            if ( j % scanLineLength >= 2 ) {
                a = ( image[ j - 2 ] & 0x000000ff );
            }

            if ( j >= scanLineLength ) {
                b = ( image[ j - scanLineLength ] & 0x000000ff );
            }

            if ( j % scanLineLength >= 2 && j >= scanLineLength) {
                c = ( image[ j - ( scanLineLength + 2 ) ] & 0x000000ff );
            }

            applyFilters( filter, image, j, a, b, c );

            j++;
        }

        return image;
    }


    // Grayscale Image with Bit Depth == 8
    private byte[] getImageColorType0BitDepth8() {

        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        byte filter = 0x00;
        int scanLineLength = this.w;

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % ( scanLineLength + 1 ) == 0 ) {
                filter = inflated[ i ];
                continue;
            }

            image[ j ] = inflated[ i ];

            int a = 0;
            int b = 0;
            int c = 0;

            if ( j % scanLineLength >= 1 ) {
                a = ( image[ j - 1 ] & 0x000000ff );
            }

            if ( j >= scanLineLength ) {
                b = ( image[ j - scanLineLength ] & 0x000000ff );
            }

            if ( j % scanLineLength >= 1 && j >= scanLineLength) {
                c = ( image[ j - ( scanLineLength + 1 ) ] & 0x000000ff );
            }

            applyFilters( filter, image, j, a, b, c );

            j++;
        }

        return image;
    }


    // Grayscale Image with Bit Depth == 4
    private byte[] getImageColorType0BitDepth4() {

        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        int scanLineLength = this.w / 2 + 1;
        if ( this.w % 2 > 0 ) {
            scanLineLength += 1;
        }

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % scanLineLength == 0 ) {
                continue;
            }

            image[ j++ ] = inflated[ i ];
        }

        return image;
    }


    // Grayscale Image with Bit Depth == 2
    private byte[] getImageColorType0BitDepth2() {

        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        int scanLineLength = this.w / 4 + 1;
        if ( this.w % 4 > 0 ) {
            scanLineLength += 1;
        }

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % scanLineLength == 0 ) {
                continue;
            }

            image[ j++ ] = inflated[ i ];
        }

        return image;
    }


    // Grayscale Image with Bit Depth == 1
    private byte[] getImageColorType0BitDepth1() {

        int j = 0;
        byte[] image = new byte[ inflated.Length - this.h ];

        int scanLineLength = this.w / 8 + 1;
        if ( this.w % 8 > 0 ) {
            scanLineLength += 1;
        }

        for ( int i = 0; i < inflated.Length; i++ ) {

            if ( i % scanLineLength == 0 ) {
                continue;
            }

            image[ j++ ] = inflated[ i ];
        }

        return image;
    }


    private void applyFilters( byte filter, byte[] image, int j, int a, int b, int c ) {

        if ( filter == 0x00 ) {             // None
            // Nothing to do.
        }
        else if ( filter == 0x01 ) {        // Sub
            image[ j ] += ( byte ) a;
        }
        else if ( filter == 0x02 ) {        // Up
            image[ j ] += ( byte ) b;
        }
        else if ( filter == 0x03 ) {        // Average
            image[ j ] += ( byte ) Math.Floor( ( double ) ( a + b ) / 2 );
        }
        else if ( filter == 0x04 ) {        // Paeth
            int pr = 0;
            int p = a + b - c;
            int pa = Math.Abs( p - a );
            int pb = Math.Abs( p - b );
            int pc = Math.Abs( p - c );
            if ( pa <= pb && pa <= pc ) {
                pr = a;
            }
            else if ( pb <= pc ) {
                pr = b;
            }
            else {
                pr = c;
            }

            image[ j ] += ( byte ) ( pr & 0x000000ff );
        }

    }


    private byte[] GetDecompressedData() {
        Decompressor decompressor = new Decompressor(data);
        return decompressor.GetDecompressedData();
    }


    private byte[] DeflateReconstructedData() {
        Compressor compressor = new Compressor(image);
        return compressor.GetCompressedData();
    }


    private byte[] AppendIdatChunk(byte[] array1, byte[] array2) {
        if (array1 == null) {
            return array2;
        } else if (array2 == null) {
            return array1;
        }
        byte[] joinedArray = new byte[array1.Length + array2.Length];
        Array.Copy(array1, 0, joinedArray, 0, array1.Length);
        Array.Copy(array2, 0, joinedArray, array1.Length, array2.Length);
        return joinedArray;
    }

/*
    public static void Main(String[] args) {
        FileStream fis = new FileStream(args[0], FileMode.Open, FileAccess.Read);
        PNGImage png = new PNGImage(fis);
        byte[] image = png.GetData();
        byte[] alpha = png.GetAlpha();
        int w = png.GetWidth();
        int h = png.GetHeight();
        int c = png.GetColorType();
        fis.Dispose();

        String fileName = args[0].Substring(0, args[0].LastIndexOf("."));
        FileStream fos = new FileStream(fileName + ".jet", FileMode.Create);
        BufferedStream bos = new BufferedStream(fos);
        WriteInt(w, bos);           // Width
        WriteInt(h, bos);           // Height
        bos.WriteByte((byte) c);    // Color Space
        if (alpha != null) {
            bos.WriteByte((byte) 1);
            WriteInt(alpha.Length, bos);
            bos.Write(alpha, 0, alpha.Length);
        }
        else {
            bos.WriteByte((byte) 0);
        }
        WriteInt(image.Length, bos);
        bos.Write(image, 0, image.Length);
        bos.Flush();
        bos.Dispose();
    }


    private static void WriteInt(int i, Stream os) {
        os.WriteByte((byte) (i >> 24));
        os.WriteByte((byte) (i >> 16));
        os.WriteByte((byte) (i >>  8));
        os.WriteByte((byte) (i >>  0));
    }
*/

}   // End of PNGImage.cs
}   // End of namespace PDFjet.NET
