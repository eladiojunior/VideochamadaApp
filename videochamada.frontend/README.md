## Solução de Videochamada para Atendimento de Clientes

### Visão Geral
Desenvolvemos uma solução moderna e eficiente de videochamada destinada ao atendimento online, oferecendo um canal direto e seguro entre **clientes** e **profissionais de saúde**.

### Funcionalidades e Fluxo do Cliente

#### Área do Cliente
1. **Registro:** Inicialmente, o cliente realiza um cadastro simplificado e seguro na plataforma.
2. **Consulta:** Caso o cliente já tenha um cadastro, será possível consultar o cliente e continuar a utilização da plataforma. 
2. **Teste de Dispositivos:** Antes de entrar na fila de atendimento, o cliente tem a oportunidade de testar seus dispositivos (câmera e microfone), garantindo que a videochamada ocorra sem interrupções ou problemas técnicos.
3. **Termo de Aceite de Uso:** Será necessário que o cliente leia e aceite um termo de aceite de uso da solução de videochamada e asseguras que o cliente está ciente dos termos.
4. **Fila de Espera:** Uma vez registrado e após a verificação dos dispositivos, o cliente entra em uma fila de espera digital. A plataforma informa ao paciente sua posição na fila.
4. **Atendimento:** Quando chega a vez do cliente, ele é conectado imediatamente a um profissional de saúde para um atendimento via videochamada.
5. **Avaliação:** Após o término do atendimento, o cliente tem a opção de avaliar seu atendimento, fornecendo feedback valioso para os profissionais e para a melhoria contínua da plataforma.

#### Área do Profissional de Saúde
1. **Registro:** Para realizar os atendimento será necessário realizar um cadastro simplificado e seguro da equipe de saúde na plataforma.
2. **Acesso:** Possibilida o acesso a área da equipe de saúde para realizar os atendimentos dos clientes que estão na fila e gestão dos profissionais de saúde.
3. **Atendimento:** Na área do profissional de saúde ele é acionado para atender um cliente que está na fila de atendimento por videochamada.

#### Painel de Gestão
Área com os paineis de acompanhamento dos atendimento, clientes e profissionais de saúde na plataforma.

### Tecnologia Utilizada

- **WebRTC:** Para transmissão de áudio e vídeo em tempo real, proporcionando conexões diretas e seguras.
- **.Net Core:** Back-end robusto e escalável, facilitando a manutenção e expansão da solução.
- **Bootstrap 5:** Front-end responsivo, garantindo que a plataforma seja acessível e funcional em diversos dispositivos, desde smartphones a computadores.

### Benefícios para o Cliente

- **Conveniência:** Os clientes podem ser atendidos por profissionais de saúde sem sair de casa, reduzindo deslocamentos, esperas em ambientes hospitalares e exposição a ambientes potencialmente insalubres.
- **Segurança:** As conexões são criptografadas, garantindo a privacidade e a segurança dos dados dos cliente.
- **Flexibilidade:** A solução é compatível com uma variedade de dispositivos, permitindo que o cliente escolha o que for mais conveniente para ele.
- **Feedback em Tempo Real:** A avaliação pós-atendimento permite que os profissionais de saúde e os administradores da plataforma recebam feedback imediato, ajudando a aprimorar o atendimento e a experiência do usuário.

#### ATENÇÃO! Esse projeto não armazena dados em banco de dados, pois é um projeto para testar o processo de atendimento de um cliente por um profissional de saúde utilizando a videochamada, todos os fluxos e solução NÃO DEVE ser utilizada em ambiente de produção até que seja implementada soluções de segurança para acesso do cliente e profissional de saúde.