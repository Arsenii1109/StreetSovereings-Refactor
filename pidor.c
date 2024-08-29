#include <stdio.h>
#include <stdlib.h>
int main() {
    char* stroke;

    gets("%s", stroke, sizeof(stroke));
    printf("%s", stroke);

    return 0;
}