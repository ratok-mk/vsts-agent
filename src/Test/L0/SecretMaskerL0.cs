using Xunit;

namespace Microsoft.VisualStudio.Services.Agent.Tests
{
    public sealed class SecretMaskerL0
    {
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_EmptyInput()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("abcd"));

            var result = secretMasker.MaskSecrets(null);
            Assert.Equal(string.Empty, result);

            result = secretMasker.MaskSecrets(string.Empty);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_NoMasks()
        {
            var secretMasker = new SecretMasker();
            var input = "abcdefg";
            var result = secretMasker.MaskSecrets(input);
            Assert.Equal(input, result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_BasicReplacement()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("def"));

            var input = "abcdefg";
            var result = secretMasker.MaskSecrets(input);

            Assert.Equal("abc********g", result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_MultipleInstances()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("def"));

            var input = "abcdefgdef";
            var result = secretMasker.MaskSecrets(input);

            Assert.Equal("abc********g********", result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_MultipleAdjacentInstances()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("abc"));

            var input = "abcabcdef";
            var result = secretMasker.MaskSecrets(input);

            Assert.Equal("********def", result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_MultipleSecrets()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("bcd"));
            secretMasker.Add(new ValueSecret("fgh"));

            var input = "abcdefghi";
            var result = secretMasker.MaskSecrets(input);

            Assert.Equal("a********e********i", result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_OverlappingSecrets()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("def"));
            secretMasker.Add(new ValueSecret("bcd"));

            var input = "abcdefg";
            var result = secretMasker.MaskSecrets(input);

            // a naive replacement would replace "def" first, and never find "bcd", resulting in "abc********g"
            // or it would replace "bcd" first, and never find "def", resulting in "a********efg"

            Assert.Equal("a********g", result);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void SecretMasker_MaskSecrets_AdjacentSecrets()
        {
            var secretMasker = new SecretMasker();
            secretMasker.Add(new ValueSecret("efg"));
            secretMasker.Add(new ValueSecret("bcd"));

            var input = "abcdefgh";
            var result = secretMasker.MaskSecrets(input);

            // two adjacent secrets are basically one big secret

            Assert.Equal("a********h", result);
        }
    }
}