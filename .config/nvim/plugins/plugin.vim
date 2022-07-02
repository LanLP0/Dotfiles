call plug#begin()
Plug 'folke/tokyonight.nvim'
Plug 'nvim-lualine/lualine.nvim'
Plug 'terrortylor/nvim-comment'
Plug 'kyazdani42/nvim-web-devicons'
Plug 'vifm/vifm.vim'
Plug 'preservim/nerdtree'
Plug 'Xuyuanp/nerdtree-git-plugin'
Plug 'tiagofumo/vim-nerdtree-syntax-highlight'
Plug 'PhilRunninger/nerdtree-buffer-ops'
Plug 'ap/vim-css-color'
call plug#end()

let confs = glob(fnamemodify(resolve(expand('<sfile>:p')), ':h') . "/configs/*", 0, 1)

for conf in confs
    execute "source " . conf
endfor
