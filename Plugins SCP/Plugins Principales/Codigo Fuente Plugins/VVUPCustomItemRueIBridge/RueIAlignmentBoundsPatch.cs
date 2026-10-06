using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace VVUPCustomItemRueIBridge
{
    [HarmonyPatch]
    internal static class RueIAlignmentBoundsPatch
    {
        private static MethodInfo _splitUntilMethod;

        private static MethodBase TargetMethod()
        {
            Type combinerType = AccessTools.TypeByName("RueI.API.Parsing.ElementCombiner");
            MethodInfo combineMethod = AccessTools.Method(combinerType, "Combine");
            _splitUntilMethod = AccessTools.Method(combinerType, "SplitUntil");

            if (combineMethod == null || _splitUntilMethod == null)
                throw new InvalidOperationException("RueI's expected element-combining methods could not be found.");

            return combineMethod;
        }

        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            LocalBuilder positionLocal = generator.DeclareLocal(typeof(int));
            MethodInfo clampMethod = AccessTools.Method(
                typeof(RueIAlignmentBoundsPatch),
                nameof(ClampPosition));
            int guardedCalls = 0;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.operand is MethodInfo calledMethod &&
                    calledMethod.IsGenericMethod &&
                    calledMethod.GetGenericMethodDefinition() == _splitUntilMethod)
                {
                    Type spanType = calledMethod.GetParameters()[0].ParameterType.GetElementType();
                    MethodInfo lengthGetter = spanType?.GetProperty("Length")?.GetGetMethod();

                    if (lengthGetter == null || clampMethod == null)
                        throw new InvalidOperationException("RueI's SplitUntil bounds could not be guarded.");

                    var savePosition = new CodeInstruction(OpCodes.Stloc, positionLocal);
                    savePosition.labels.AddRange(instruction.labels);
                    savePosition.blocks.AddRange(instruction.blocks);
                    instruction.labels.Clear();
                    instruction.blocks.Clear();

                    yield return savePosition;
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Call, lengthGetter);
                    yield return new CodeInstruction(OpCodes.Ldloc, positionLocal);
                    yield return new CodeInstruction(OpCodes.Call, clampMethod);
                    guardedCalls++;
                }

                yield return instruction;
            }

            if (guardedCalls == 0)
                throw new InvalidOperationException("RueI's element combiner no longer calls the expected SplitUntil method.");

            Exiled.API.Features.Log.Info($"Installed bounds guards for {guardedCalls} RueI alignment parser call(s).");
        }

        private static int ClampPosition(int length, int position)
        {
            if (position < 0)
                return 0;

            return position > length ? length : position;
        }
    }
}
