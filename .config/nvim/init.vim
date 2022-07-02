set ic
set expandtab
set shiftwidth=4
set smartcase
set nobackup
set mouse=a
set tabstop=4
set number
set laststatus=3

" Load plugins
execute "source " . fnamemodify(resolve(expand('<sfile>:p')), ':h') . "/plugins/plugin.vim"

" Load configs
let confs = glob(fnamemodify(resolve(expand('<sfile>:p')), ':h') . "/configs/*", 0, 1)

for conf in confs
    execute "source " . conf
endfor
