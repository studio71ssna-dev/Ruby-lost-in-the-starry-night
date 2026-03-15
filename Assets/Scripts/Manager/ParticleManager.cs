using UnityEngine;
using Singleton;
using System.Collections.Generic;
using System.Collections;

namespace SingletonManagers
{
    public class ParticleManager : SingletonPersistent
    {
        public static ParticleManager Instance => GetInstance<ParticleManager>();

        [System.Serializable]
        public class ParticlePrefab
        {
            public string name;
            public ParticleSystem prefab;
        }

        [SerializeField] private List<ParticlePrefab> particlePrefabs;
        public List<ParticlePrefab> ParticlePrefabs => particlePrefabs;

        private readonly Dictionary<string, ParticleSystem> _prefabDictionary = new Dictionary<string, ParticleSystem>();
        private readonly Dictionary<string, Queue<ParticleSystem>> _particlePools = new Dictionary<string, Queue<ParticleSystem>>();

        private void Start()
        {
            foreach (var entry in particlePrefabs)
            {
                if (entry.prefab != null)
                {
                    _prefabDictionary[entry.name] = entry.prefab;
                    _particlePools[entry.name] = new Queue<ParticleSystem>();

                    for (int i = 0; i < 2; i++)
                    {
                        ParticleSystem particle = Instantiate(entry.prefab, transform);
                        particle.gameObject.SetActive(false);
                        _particlePools[entry.name].Enqueue(particle);
                    }
                }
            }
        }
        private void PlayParticle(string particleName, Vector3 position, Quaternion rotation, Color color)
        {
            if (!_prefabDictionary.ContainsKey(particleName))
            {
                Debug.LogWarning($"Particle '{particleName}' not found!");
                return;
            }

            string key = particleName;
            ParticleSystem particle;

            if (_particlePools[key].Count > 0)
            {
                particle = _particlePools[key].Dequeue();
                particle.transform.position = position;
                particle.transform.rotation = rotation;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = Instantiate(_prefabDictionary[key], position, rotation);
            }

            // Apply color
            var main = particle.main;
            main.startColor = color;

            particle.Play();
            StartCoroutine(ReturnToPool(particle, key, particle.main.duration));
        }
        public void PlayParticle(string particleName, Vector3 position, Color color)
        {
            PlayParticle(particleName, position, Quaternion.identity, color);
        }
        public void PlayParticle(string particleName, Vector3 position)
        {
            PlayParticle(particleName, position, Quaternion.identity, Color.white);
        }

        private IEnumerator ReturnToPool(ParticleSystem particle, string key, float delay)
        {
            yield return new WaitForSeconds(delay);
            particle.Stop();
            particle.gameObject.SetActive(false);
            _particlePools[key].Enqueue(particle);
        }
    }
}