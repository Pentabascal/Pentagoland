using System;
using System.Collections.Generic;
using VoiceChat.Codec;
using VoiceChat.Networking;

namespace SwiftArcadeMode.Utils.Voice
{
    public static class VoiceDecoding
    {
        private static readonly Dictionary<ReferenceHub, OpusDecoder> Decoders = [];

        private static readonly float[] ReadBuffer = new float[24000];

        public static ArraySegment<float> ToPcm(this VoiceMessage message)
        {
            if (!Decoders.TryGetValue(message.Speaker, out OpusDecoder decoder))
            {
                decoder = Decoders[message.Speaker] = new OpusDecoder();
            }

            int len = decoder.Decode(message.Data, message.DataLength, ReadBuffer);

            return ReadBuffer.Segment(0, len);
        }

        public static float CalculateRMS(float[] pcmData)
        {
            if (pcmData == null || pcmData.Length == 0)
                return 0f;

            double sumOfSquares = 0.0;

            foreach (float sample in pcmData)
            {
                sumOfSquares += sample * sample;
            }

            double meanSquare = sumOfSquares / pcmData.Length;
            return (float)Math.Sqrt(meanSquare);
        }

        public static float CalculateLoudnessDB(float[] pcmData)
        {
            if (pcmData == null || pcmData.Length == 0)
                return -96f; // Silence in dB

            float rms = CalculateRMS(pcmData);

            // Convert to dBFS (decibels relative to full scale)
            // Avoid log(0) by using a very small value
            if (rms < 1e-6f)
                return -96f;

            return 20f * (float)Math.Log10(rms);
        }
    }
}
