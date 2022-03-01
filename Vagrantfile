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
            provider.size = 's-1vcpu-1gb'
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

            nohup docker-compose -f docker-compose-prod.yml up -d
        SHELL
    end
end
