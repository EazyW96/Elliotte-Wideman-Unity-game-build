#!/bin/bash
set -e

echo "=== Unity Git LFS setup starting ==="

# Step 1: .gitignore
cat > .gitignore <<'EOF'
# OS
.DS_Store

# Unity caches/builds (never commit)
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild*/
[Ll]ogs/
UserSettings/
**/Bee/
**/BurstCache/
**/Artifacts/
[Mm]emoryCaptures/
[Cc]rashReports/
[Ss]ysinfo/

# IDE
.vs/
.idea/
.vscode/
*.DotSettings.user

# Misc
*.pid
EOF
echo ".gitignore created."

# Step 2: Git LFS setup
git lfs install --local
git lfs track "*.png" "*.jpg" "*.jpeg" "*.tga" "*.psd" "*.tif" "*.tiff" "*.exr" "*.hdr"
git lfs track "*.mp3" "*.wav" "*.ogg" "*.flac" "*.aac"
git lfs track "*.fbx" "*.obj" "*.blend" "*.gltf" "*.glb"
git lfs track "*.mp4" "*.mov"
git lfs track "*.ttf" "*.otf"
git add .gitattributes
echo "LFS tracking rules applied."

# Step 3: Remove cached junk
git rm -r --cached GameUp/Library Space_Quest/Library GameUp/Temp Space_Quest/Temp \
  GameUp/UserSettings Space_Quest/UserSettings .DS_Store 2>/dev/null || true

# Step 4: Restage existing binaries for LFS
git ls-files -z | grep -zE '\.(png|jpe?g|tga|psd|tiff?|exr|hdr|mp3|wav|ogg|flac|aac|fbx|obj|blend|gltf|glb|mp4|mov|ttf|otf)$' \
  | xargs -0 git rm --cached -- 2>/dev/null || true

git add .
git commit -m "Auto-setup Unity .gitignore and Git LFS; stop tracking cache files"

echo "=== Setup complete ==="
echo "Next:  git push origin Demo-branch"

