﻿using System;
using System.Collections.Generic;

using CBSL.Core.Collections.Compressed;

using CodeBlaze.Vloxy.Engine.Extensions;

using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Data {

    public class CompressibleChunkData<B> : IChunkData<B> where B : IBlock {

        private Vector3Int _chunkSize;

        private CompressedArray<B> _data;
        
        public CompressibleChunkData(B[] data, int dataSize, Vector3Int chunkSize, Func<byte[], B> fromBytes, Func<B, byte[]> getBytes) {
            _chunkSize = chunkSize;
            _data = new CompressedArray<B>(data, dataSize, fromBytes, getBytes);
        }

        public CompressibleChunkData(List<byte> bytes, int dataSize, Vector3Int chunkSize, Func<byte[], B> fromBytes, Func<B, byte[]> getBytes) {
            _chunkSize = chunkSize;
            _data = new CompressedArray<B>(bytes, _chunkSize.Size(),  dataSize, fromBytes, getBytes);
        }

        public CompressedArray<B>.DataState State => _data.State;

        public void Compress() {
            if (State == CompressedArray<B>.DataState.COMPRESSED) return;
            _data.Compress();
        }

        public void DeCompress() {
            if (State == CompressedArray<B>.DataState.DECOMPRESSED) return;
            _data.DeCompress();
        }

        public void SetBlock(B block, int x, int y, int z) => _data.SetAt(_chunkSize.Flatten(x, y, z), block);

        public B GetBlock(int x, int y, int z) => _data.GetAt(_chunkSize.Flatten(x, y, z));

        public void ForEach(Action<B> opt) {
            for (int i = 0; i < _data.Length; i++) {
                opt(_data.GetAt(i));
            }
        }

    }

}