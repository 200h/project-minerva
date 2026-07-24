# Unity Verification Tools

Project Minerva's local Unity 5.6.7f1 validation entry point is:

```bash
./Tools/Verification/verify-unity56-editmode.sh
```

Run the preflight alone with:

```bash
./Tools/Verification/unity56-preflight.sh
```

The optional worktree helpers isolate Unity-generated artifacts from an active
checkout:

```bash
./Tools/Verification/create-unity-worktree.sh <ref>
./Tools/Verification/remove-unity-worktree.sh <path>
```

See [`../../Docs/Runbooks/UNITY_5_6_VALIDATION.md`](../../Docs/Runbooks/UNITY_5_6_VALIDATION.md)
for prerequisites, evidence interpretation, and recovery steps.
