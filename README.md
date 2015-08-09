# Unbreakable
Using a text file as cypher (for example a certain book from the Gutenberg project) this simple tool encrypts a input file. By setting an offset the book used
at cypher key unlimited varieties can be used. This cypher is only breakable if the attacker knows the book and offset.

# Debug commandline options
To encrypt: -e -i ../../../Files/plain.txt -o ../../../Files/result.txt -k ../../../Files/pg27468.txt -f 1500
To decrypt: -d -i ../../../Files/result.txt -o ../../../Files/plain.txt -k ../../../Files/pg27468.txt -f 1500