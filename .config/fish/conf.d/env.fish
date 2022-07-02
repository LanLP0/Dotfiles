set -gx MANPAGER 'nvim -c "%! col -b" -c "set ft=man nomod | let &titlestring=$MAN_PN"'
set -gx EDITOR nvim
set -gx PATH $PATH:/home/lanlp/.dotnet/tools/
