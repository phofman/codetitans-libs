
GiT clone of the CodeTitans Library project

Available at:
    http://codetitans.codeplex.com

Aim:
    Simplify management for GiT source code references.
 As the original project is hosted using SVN, this one
 will allow adding into existing GiT repositories
 a regular submodule.
    The development is still done at original location. 

Repository created by:
    1) git svn init https://codetitans.svn.codeplex.com/svn/trunk -R codetitans --prefix=codetitans/
    2) git svn fetch codetitans -r 22968
    3) and merged:
        'remotes/codetitans/git-svn' into 'master'

Usage:
    git submodule add git@github.com:phofman/codetitans-libs.git externals/CodeTitans

and later
    git submodule update

=======================================================
                                        CodeTitans 2013
