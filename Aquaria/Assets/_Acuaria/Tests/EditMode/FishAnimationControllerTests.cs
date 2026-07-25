using Acuaria.Fish;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class FishAnimationControllerTests
    {
        [TestCase("Assets/_Acuaria/Prefabs/Fish/Fish_Guppy.prefab")]
        [TestCase("Assets/_Acuaria/Prefabs/Fish/Fish_Clownfish.prefab")]
        public void Adapter_IsVisualOnlyAndOwnedByAcuaria(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.GetComponent<FishMovement>(), Is.Not.Null);
            Assert.That(prefab.GetComponent<FishAnimationController>(), Is.Not.Null);
            Assert.That(prefab.transform.Find("Visual"), Is.Not.Null);
            Assert.That(prefab.GetComponentInChildren<Animator>(true), Is.Not.Null);
            Assert.That(prefab.GetComponentInChildren<Animator>(true).applyRootMotion, Is.False);
            Assert.That(prefab.GetComponentsInChildren<Rigidbody>(true), Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<Collider>(true), Is.Empty);
            Assert.That(prefab.GetComponentsInChildren<MonoBehaviour>(true),
                Has.All.Matches<MonoBehaviour>(value =>
                    value is FishMovement || value is FishAnimationController));
        }

        [Test]
        public void RealSpecies_UseDistinctAdapterPrefabs()
        {
            FishSpecies guppy = AssetDatabase.LoadAssetAtPath<FishSpecies>(
                "Assets/_Acuaria/Data/Fish/GuppyPrototype.asset");
            FishSpecies clownfish = AssetDatabase.LoadAssetAtPath<FishSpecies>(
                "Assets/_Acuaria/Data/Fish/ClownfishPrototype.asset");
            Assert.That(guppy, Is.Not.Null);
            Assert.That(clownfish, Is.Not.Null);
            Assert.That(guppy.SpeciesId, Is.Not.EqualTo(clownfish.SpeciesId));
            Assert.That(guppy.VisualPrefab, Is.Not.Null);
            Assert.That(clownfish.VisualPrefab, Is.Not.Null);
            Assert.That(guppy.VisualPrefab, Is.Not.SameAs(clownfish.VisualPrefab));
            Assert.That(AssetDatabase.GetAssetPath(guppy.VisualPrefab),
                Does.StartWith("Assets/_Acuaria/Prefabs/Fish/"));
            Assert.That(AssetDatabase.GetAssetPath(clownfish.VisualPrefab),
                Does.StartWith("Assets/_Acuaria/Prefabs/Fish/"));
        }
    }
}
