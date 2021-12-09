﻿using System.Collections.Generic;

using CodeBlaze.Vloxy.Engine.Behaviour;
using CodeBlaze.Vloxy.Engine.Settings;
using CodeBlaze.Vloxy.Engine.Utils.Extensions;
using CodeBlaze.Vloxy.Engine.Utils.Logger;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Pool;

namespace CodeBlaze.Vloxy.Engine.Components {

    public class ChunkBehaviourPool {
        
        private IObjectPool<ChunkBehaviour> _pool;

        private Dictionary<int3, ChunkBehaviour> _active;

        public ChunkBehaviourPool(Transform transform, VoxelSettings settings) {
            var viewRegionSize = settings.Chunk.DrawDistance.CubedSize();
            
            _active = new Dictionary<int3, ChunkBehaviour>(viewRegionSize);
            
            _pool = new ObjectPool<ChunkBehaviour>( // pool size = x^2 + 1
                () => {
                    var go = new GameObject("Chunk", typeof(ChunkBehaviour)) {
                        transform = {
                            parent = transform
                        }
                    };
                    
                    go.SetActive(false);
            
                    var chunkBehaviour = go.GetComponent<ChunkBehaviour>();
                    
                    chunkBehaviour.SetRenderSettings(settings.Renderer);

                    return chunkBehaviour;
                },
                chunkBehaviour => chunkBehaviour.gameObject.SetActive(true),
                chunkBehaviour => chunkBehaviour.gameObject.SetActive(false),
                null, false, viewRegionSize, viewRegionSize
            );
            
            VloxyLogger.Info<ChunkBehaviourPool>("Initialized Size : " + viewRegionSize);
        }
        
        public ChunkBehaviour Claim(int3 pos) {
            var behaviour = _pool.Get();

            behaviour.transform.position = pos.GetVector3();
            behaviour.name = $"Chunk({pos})";

            _active.Add(pos, behaviour);
            
            return behaviour;
        }

        public void Reclaim(int3 pos) {
            _pool.Release(_active[pos]);
            _active.Remove(pos);
        }

    }

}