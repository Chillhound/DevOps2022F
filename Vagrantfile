Vagrant.configure('2') do |config|
    config.vm.synced_folder ".", "/vagrant", type: "rsync"
        
    config.vm.define "minitwit" do |server|
        server.vm.provider :digital_ocean do |provider, override|
            override.ssh.private_key_path = '~/.ssh/id_rsa'
            override.vm.box = 'digital_ocean'
            override.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
            override.nfs.functional = false
            override.vm.allowed_synced_folder_types = :rsync
            provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
            provider.image = 'ubuntu-18-04-x64'
            provider.region = 'fra1'
            provider.size = 's-1vcpu-2gb'
            provider.backups_enabled = false
            provider.private_networking = false
            provider.ipv6 = false
            provider.monitoring = false
        end

        server.vm.provision "shell", inline: <<-SHELL
            echo "Installing Docker"
            curl -fsSL https://get.docker.com -o get-docker.sh 
            sudo sh get-docker.sh

            echo "Installing Docker-compose"
            sudo curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
            sudo chmod +x /usr/local/bin/docker-compose

            cp -r /vagrant/* $HOME

            sudo echo "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABgQDjq+nlPcyzU3DUnzzOAGAMeh729F0RJGMHaYj7N5+q1ndU3qE9RuLEfb2nhFcN28ayq9nI07rWN/zQkgpSpCidb3Wm7uIPLx4gSpyWtpsv6YFeuZKA7aErKcjB/FqR6J83enb1nxo9zc1kM7hKz3omzLUwnWMnuz90MD3JcOzcl7GWn4j7rVIba+FntZIp2O08hqr7ljRHd4qNR6meEwe7arbY3dxay7rjEgtfer9Vr+gpFPhIh9OPkgQcHCLv/XWnC14U28XHuYhvI3iQGUlvuorpPGcJy/U8Z4xxwERtkOYfCI6mXooL2iPMdANHuq7N1UxVhhpDKbT1SqhToZQNOxvVSupLK0uoGu5eb9EoGsLMHUMsx+a9gR0TkaxnGNN4Rp/bQogAos3BVYD2wMVXRAVam8Xai+bjbFSrBPaoSV8wnStcQq33cEZZX4ObpRdeGbo3oMdUK4V+z1r7w2uvry8gdsKSzW0Pa384i0YkDaHujxQJYmST+i1Edfwd8wM= emilio@Emils-MacBook-Air.local" >> .ssh/authorized_keys

            nohup docker-compose -f docker-compose-prod.yml up -d
        SHELL
    end
end
