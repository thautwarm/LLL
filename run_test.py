
from Redy.Tools.PathLib import Path
import os

for each in Path("ir-snippets").list_dir(lambda x: x.endswith('.ll')):
    os.system(f"lli-6.0 {each}")
    os.system("echo $?")
