# Changelog

## Unreleased

- Align LabAPI data, plugin, dependency, and config roots with the official `PathManager`, including `gamedir_for_configs`.
- Mark the plugin as transparent to LabAPI only while configuration writes remain disabled.
- Redact multiline secret blocks in log tails and tolerate damaged backup manifests in inventory output.
- Reject oversized or malformed backups before hashing them during restore.
- Expand isolated tests for hoster paths, log redaction, damaged manifests, and backup-size limits.
- Add scoped, backup-protected scalar editing for SCP:SL `config_gameplay.txt` using `clave: valor` syntax.
- Traverse nested plugin and dependency folders with explicit inventory limits, including custom LabAPI paths beneath known roots.
- Make plugin/dependency inventory roots configurable so custom `AdditionalRoots` can be checked without scanning arbitrary disks.
- Add live runtime health snapshots and bounded, redacted support reports with recent server log tails.
- Register the diagnostics command in the dedicated server console as well as Remote Admin, and add VS Code build/test tasks.
- Add a read-only validator for the SCP:SL public-verification IP/bind settings documented by the support wiki.
- Discover the standard SCP:SL per-port config directory and document integrations that LabAPI exposes through its official path/config APIs.
- Add on-demand static plugin DLL inspection with SHA-256 and declared file-version metadata without loading plugin code.
- Add bounded static .NET assembly-reference inspection for plugins, with explicit reporting of unresolved references, differing versions, and analysis limits.
- Resolve LabAPI data, config, plugin, and dependency roots from the official `PathManager`; inspect configured `PluginPaths` and `DependencyPaths` with port substitution while rejecting paths outside their respective roots.
- Derive the SCP:SL configuration root from LabAPI's active `GameplayConfigPath` rather than assuming the per-port directory layout.
- Prevent the YAML scalar editor from mistaking mapping fields inside YAML sequences for ordinary nested keys.
- Clarify compatibility reports when framework assemblies are not in the scanned folders, bound assembly metadata inspection, and report unreadable DLL candidates.
- Add a read-only live inventory of plugins enabled/known and dependencies registered by LabAPI; include it in on-demand support reports.

## 1.0.0

- Added admin-only Remote Admin commands for diagnostics, plugin-file inventory, bounded file reading, and log tails.
- Added configurable read roots and a separate explicit write allowlist.
- Added redacted previews, YAML scalar edits, hash-verified backups, rollback restore, and audit entries.
- Added isolated safety/workflow tests and GitHub Actions validation.
