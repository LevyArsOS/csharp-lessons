using CSharpLessons.StructVsClass.Data;
using Xunit;

namespace CSharpLessons.Tests.StructVsClass
{
    public class StructVsClassTests
    {
        [Fact]
        public void VectorArithmetic_ProducesIdenticalResults()
        {
            var vc1 = new Vec2Class(12.5f, -4.25f);
            var vc2 = new Vec2Class(3.5f, 8.25f);

            var vs1 = new Vec2Struct(12.5f, -4.25f);
            var vs2 = new Vec2Struct(3.5f, 8.25f);

            var vcAdd = vc1 + vc2;
            var vsAdd = vs1 + vs2;

            Assert.Equal(vcAdd.X, vsAdd.X);
            Assert.Equal(vcAdd.Y, vsAdd.Y);

            var vcScale = vc1 * 2.5f;
            var vsScale = vs1 * 2.5f;

            Assert.Equal(vcScale.X, vsScale.X);
            Assert.Equal(vcScale.Y, vsScale.Y);

            Assert.Equal(vc1.LengthSquared(), vs1.LengthSquared());
            Assert.Equal(vc2.LengthSquared(), vs2.LengthSquared());
        }

        [Fact]
        public void StructEquality_WithoutBoxing_WorksExpectedly()
        {
            var s1 = new Vec2Struct(10f, 20f);
            var s2 = new Vec2Struct(10f, 20f);
            var s3 = new Vec2Struct(10.01f, 20f);

            Assert.True(s1.Equals(s2));
            Assert.False(s1.Equals(s3));
            Assert.True(s1 == s2);
            Assert.True(s1 != s3);
            Assert.Equal(s1.GetHashCode(), s2.GetHashCode());

            Assert.True(s1.Equals((object)s2));
            Assert.False(s1.Equals((object)s3));
            Assert.False(s1.Equals(null));
            Assert.False(s1.Equals("not-a-struct"));
        }

        [Fact]
        public void EntityTransformSimulator_ProducesEquivalentResults()
        {
            const int count = 250;
            const float dt = 0.016f;

            var (cPos, cVel) = EntityTransformSimulator.CreateClassEntities(count);
            var (sPos, sVel) = EntityTransformSimulator.CreateStructEntities(count);

            EntityTransformSimulator.IntegrateClass(cPos, cVel, dt);
            EntityTransformSimulator.IntegrateStruct(sPos, sVel, dt);

            for (int i = 0; i < count; i++)
            {
                Assert.Equal(cPos[i].X, sPos[i].X, 5);
                Assert.Equal(cPos[i].Y, sPos[i].Y, 5);
            }

            var cSum = EntityTransformSimulator.SumClass(cPos);
            var sSum = EntityTransformSimulator.SumStruct(sPos);

            Assert.Equal(cSum.X, sSum.X, 4);
            Assert.Equal(cSum.Y, sSum.Y, 4);
        }

        [Fact]
        public void EntityTransformSimulator_HandlesZeroCountAndMismatches()
        {
            var (emptyPosC, emptyVelC) = EntityTransformSimulator.CreateClassEntities(0);
            var (emptyPosS, emptyVelS) = EntityTransformSimulator.CreateStructEntities(0);

            EntityTransformSimulator.IntegrateClass(emptyPosC, emptyVelC, 0.016f);
            EntityTransformSimulator.IntegrateStruct(emptyPosS, emptyVelS, 0.016f);

            var cSumEmpty = EntityTransformSimulator.SumClass(emptyPosC);
            var sSumEmpty = EntityTransformSimulator.SumStruct(emptyPosS);

            Assert.Equal(0f, cSumEmpty.X);
            Assert.Equal(0f, cSumEmpty.Y);
            Assert.Equal(0f, sSumEmpty.X);
            Assert.Equal(0f, sSumEmpty.Y);

            // Mismatched lengths
            var posArr = new Vec2Struct[] { new(1f, 2f), new(3f, 4f) };
            var velArr = new Vec2Struct[] { new(0.5f, 0.5f) };
            EntityTransformSimulator.IntegrateStruct(posArr, velArr, 1.0f);

            Assert.Equal(1.5f, posArr[0].X);
            Assert.Equal(2.5f, posArr[0].Y);
            Assert.Equal(3f, posArr[1].X);
            Assert.Equal(4f, posArr[1].Y);
        }

        [Fact]
        public void EntityTransformSimulator_HandlesExtremeCoordinates()
        {
            var posClass = new Vec2Class[] { new(1e20f, -1e20f) };
            var velClass = new Vec2Class[] { new(1e18f, -1e18f) };
            var posStruct = new Vec2Struct[] { new(1e20f, -1e20f) };
            var velStruct = new Vec2Struct[] { new(1e18f, -1e18f) };

            EntityTransformSimulator.IntegrateClass(posClass, velClass, 0.5f);
            EntityTransformSimulator.IntegrateStruct(posStruct, velStruct, 0.5f);

            Assert.Equal(posClass[0].X, posStruct[0].X);
            Assert.Equal(posClass[0].Y, posStruct[0].Y);
        }
    }
}
