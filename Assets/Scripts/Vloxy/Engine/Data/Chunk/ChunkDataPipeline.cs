﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CodeBlaze.Vloxy.Engine.Data {

    public class ChunkDataPipeline<B> where B : IBlock {

        private List<Func<IChunkData<B>, IChunkData<B>>> Funcs;

        public ChunkDataPipeline(List<Func<IChunkData<B>, IChunkData<B>>> funcs) {
            Funcs = funcs;
        }

        public ChunkDataPipeline() {
            Funcs = new List<Func<IChunkData<B>, IChunkData<B>>>();
        }

        public void Add(Func<IChunkData<B>, IChunkData<B>> func) => Funcs.Add(func);

        public IChunkData<B> Apply(IChunkData<B> data) => Funcs.Aggregate(data, (current, func) => current == null ? null : func(current));
     
        public static class Functions {
            
            public static readonly Func<IChunkData<B>, IChunkData<B>> EmptyChunkRemover = data => {
                var ChunkSize = VoxelProvider<B>.Current.Settings.Chunk.ChunkSize;
                return ((CompressibleChunkData<B>) data).CompressedLength == 1 &&
                    data.GetBlock(ChunkSize.x - 1, ChunkSize.y - 1, ChunkSize.z - 1).IsTransparent()
                        ? null
                        : data;
            };

            public static readonly Func<IChunkData<B>, IChunkData<B>> ChunkDataCompressor = data => {
                ((CompressibleChunkData<B>) data).Compress();

                return data;
            };

            public static readonly Func<IChunkData<B>, IChunkData<B>> ChunkDataDecompressor = data => {
                ((CompressibleChunkData<B>) data).Compress();

                return data;
            };

        }
        
    }

}